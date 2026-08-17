<#
.SYNOPSIS
    Собирает расширенную статистику по проекту: файлы, строки, тесты, интерфейсы, коммиты и т.д.
.DESCRIPTION
    Анализирует JS, C#, миграции, контроллеры, сервисы, сущности, тесты, интерфейсы.
    Игнорирует папки bin, obj, node_modules.
.PARAMETER RootPath
    Путь к корню решения (по умолчанию текущая папка).
.PARAMETER OutputFile
    (Опционально) путь к файлу для сохранения отчёта.
.EXAMPLE
    .\AnalyzeProject.ps1 -RootPath "C:\Projects\Migration"
.EXAMPLE
    .\AnalyzeProject.ps1 -OutputFile "stats.txt"
#>

param(
    [string]$RootPath = ".",
    [string]$OutputFile
)

$RootPath = Resolve-Path $RootPath
Write-Host "🔍 Анализ проекта в: $RootPath" -ForegroundColor Cyan

# Исключаемые папки (бинарные, временные)
$excludeDirs = @('bin', 'obj', 'node_modules', '.vs', '.git', 'packages')

function Get-SourceFiles {
    param([string]$Path, [string]$Filter)
    Get-ChildItem -Path $Path -Filter $Filter -Recurse -File | 
        Where-Object { 
            $dir = $_.DirectoryName
            $exclude = $false
            foreach ($ex in $excludeDirs) {
                if ($dir -match "\\$ex\\" -or $dir -match "\\$ex$") {
                    $exclude = $true
                    break
                }
            }
            -not $exclude
        }
}

# Файлы и строки
$jsFiles = Get-SourceFiles -Path $RootPath -Filter "*.js"
$jsCount = $jsFiles.Count
$jsLines = 0
if ($jsCount -gt 0) {
    $jsLines = ($jsFiles | ForEach-Object { (Get-Content $_.FullName | Measure-Object -Line).Lines } | Measure-Object -Sum).Sum
}

$csFiles = Get-SourceFiles -Path $RootPath -Filter "*.cs"
$csCount = $csFiles.Count
$csLines = 0
if ($csCount -gt 0) {
    $csLines = ($csFiles | ForEach-Object { (Get-Content $_.FullName | Measure-Object -Line).Lines } | Measure-Object -Sum).Sum
}

$htmlFiles = Get-SourceFiles -Path $RootPath -Filter "*.html"
$htmlCount = $htmlFiles.Count
$cssFiles = Get-SourceFiles -Path $RootPath -Filter "*.css"
$cssCount = $cssFiles.Count

# Архитектура
$migrationFiles = $csFiles | Where-Object { 
    $_.DirectoryName -match "\\Migrations$" -and $_.Name -match "^\d{14}_" 
}
$migrationCount = $migrationFiles.Count

$serviceFiles = $csFiles | Select-String -Pattern "class\s+\w+\s*:\s*ICompanyService" -List
$serviceCount = $serviceFiles.Count

$controllerFiles = $csFiles | Select-String -Pattern "class\s+\w+Controller\s*[:{]" -List
$controllerCount = $controllerFiles.Count

# Сущности через DbSet и Fluent
$dbSetPattern = 'public\s+DbSet<([^>]+)>'
$dbSetMatches = $csFiles | Select-String -Pattern $dbSetPattern -AllMatches
$dbSetEntityNames = $dbSetMatches | ForEach-Object { $_.Matches } | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
$entityCountViaDbSet = $dbSetEntityNames.Count

$fluentTablePattern = '\.ToTable\s*\(\s*"([^"]+)"\s*\)'
$fluentMatches = $csFiles | Select-String -Pattern $fluentTablePattern -AllMatches
$fluentTableNames = $fluentMatches | ForEach-Object { $_.Matches } | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
$entityCountViaFluent = $fluentTableNames.Count

$allEntityNames = @()
$allEntityNames += $dbSetEntityNames
$allEntityNames += $fluentTableNames
$entityTotal = $allEntityNames | Sort-Object -Unique | Measure-Object | Select-Object -ExpandProperty Count

$interfaceFiles = $csFiles | Select-String -Pattern "interface\s+I\w+" -List
$interfaceCount = $interfaceFiles.Count

$dtoFiles = $csFiles | Where-Object { 
    $_.Name -match "Dto|Request|Response" -or $_.DirectoryName -match "DTO" 
}
$dtoCount = $dtoFiles.Count

# Методы в контроллерах (исправлено: используем .Path вместо .FullName)
$controllerMethodPattern = 'public\s+(async\s+)?[^\s]+\s+[^\s]+\s*\('
$controllerMethodCount = 0
if ($controllerFiles.Count -gt 0) {
    foreach ($file in $controllerFiles) {
        $filePath = $file.Path
        if (Test-Path $filePath) {
            $content = Get-Content $filePath -Raw
            $matches = [regex]::Matches($content, $controllerMethodPattern)
            $controllerMethodCount += $matches.Count
        }
    }
}

$projectFiles = Get-SourceFiles -Path $RootPath -Filter "*.csproj"
$projectCount = $projectFiles.Count

$sharedProjectFiles = $projectFiles | Where-Object { $_.Name -match "Contracts|Shared" }
$sharedProjectCount = $sharedProjectFiles.Count

# Тесты
$testFiles = $csFiles | Where-Object {
    $_.Name -match "Tests\.cs$" -or 
    (Select-String -Path $_.FullName -Pattern "\[Fact\]|\[Theory\]" -Quiet)
}
$testCount = $testFiles.Count
$testLines = 0
if ($testCount -gt 0) {
    $testLines = ($testFiles | ForEach-Object { (Get-Content $_.FullName | Measure-Object -Line).Lines } | Measure-Object -Sum).Sum
}
$coveragePercent = 0
if ($csCount -gt 0) {
    $coveragePercent = [math]::Round(($testCount / $csCount) * 100, 1)
}

# Качество
$todoCount = ($csFiles + $jsFiles | Select-String -Pattern "TODO|FIXME|HACK" -AllMatches | Measure-Object).Count

$packageRefs = Get-ChildItem -Path $RootPath -Filter "*.csproj" -Recurse | ForEach-Object {
    Select-String -Path $_.FullName -Pattern '<PackageReference Include="([^"]+)"' -AllMatches
} | ForEach-Object { $_.Matches.Groups[1].Value } | Sort-Object -Unique
$packageCount = $packageRefs.Count

# Git
$commitCount = "N/A"
$commitsLast30 = "N/A"
$authorCount = "N/A"
if (Get-Command git -ErrorAction SilentlyContinue) {
    try {
        $originalDir = Get-Location
        Set-Location $RootPath
        $commitCount = git rev-list --count HEAD 2>$null
        $commitsLast30 = git log --since="30 days ago" --oneline | Measure-Object | Select-Object -ExpandProperty Count
        $authors = git log --format='%aN' | Sort-Object -Unique
        $authorCount = $authors.Count
        Set-Location $originalDir
        if (-not $commitCount) { $commitCount = "N/A" }
        if (-not $commitsLast30) { $commitsLast30 = "N/A" }
        if (-not $authorCount) { $authorCount = "N/A" }
    } catch {
        $commitCount = "N/A"
        $commitsLast30 = "N/A"
        $authorCount = "N/A"
    }
}

$avgSize = 0
if ($csCount -gt 0) {
    $avgSize = [math]::Round($csLines / $csCount, 1)
}

# Отчёт
$report = @"
=========================================
    СТАТИСТИКА ПРОЕКТА (расширенная)
=========================================
Корень: $RootPath

📁 Файлы и строки кода:
  .js файлов       : $jsCount
  строк JS         : $jsLines
  .cs файлов       : $csCount
  строк C#         : $csLines
  средний размер .cs : $avgSize строк/файл
  .html файлов     : $htmlCount
  .css файлов      : $cssCount

🏗️  Архитектурные элементы:
  Миграций         : $migrationCount
  Сервисов (ICompanyService) : $serviceCount
  Контроллеров     : $controllerCount
  Сущностей (всего) : $entityTotal
    - через DbSet  : $entityCountViaDbSet
    - через Fluent : $entityCountViaFluent
  Интерфейсов      : $interfaceCount
  DTO/моделей      : $dtoCount
  Методов в контроллерах : $controllerMethodCount
  Проектов (.csproj) : $projectCount
  Общие библиотеки (Contracts/Shared) : $sharedProjectCount

🧪 Тестирование:
  Тестовых файлов  : $testCount
  Строк тестов     : $testLines
  Покрытие (по файлам) : $coveragePercent%

📦 Управление версиями:
  Коммитов (всего) : $commitCount
  Коммитов за 30 дней : $commitsLast30
  Уникальных авторов : $authorCount

🔧 Качество кода:
  TODO/FIXME/HACK  : $todoCount
  NuGet-пакетов    : $packageCount

=========================================
"@

Write-Host $report -ForegroundColor Green
if ($OutputFile) {
    $report | Out-File -FilePath $OutputFile -Encoding utf8
    Write-Host "✅ Отчёт сохранён в: $OutputFile" -ForegroundColor Yellow
}