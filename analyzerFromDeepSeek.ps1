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

# Функция для получения всех файлов с исключением папок
function Get-SourceFiles {
    param(
        [string]$Path,
        [string]$Filter
    )
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

# 1. JS-файлы
$jsFiles = Get-SourceFiles -Path $RootPath -Filter "*.js"
$jsCount = $jsFiles.Count
$jsLines = 0
if ($jsCount -gt 0) {
    $jsLines = ($jsFiles | ForEach-Object { (Get-Content $_.FullName | Measure-Object -Line).Lines } | Measure-Object -Sum).Sum
}

# 2. C#-файлы
$csFiles = Get-SourceFiles -Path $RootPath -Filter "*.cs"
$csCount = $csFiles.Count
$csLines = 0
if ($csCount -gt 0) {
    $csLines = ($csFiles | ForEach-Object { (Get-Content $_.FullName | Measure-Object -Line).Lines } | Measure-Object -Sum).Sum
}

# 3. Миграции (файлы в папках Migrations, имя начинается с цифр)
$migrationFiles = $csFiles | Where-Object {
    $_.DirectoryName -match "\\Migrations$" -and $_.Name -match "^\d{14}_"
}
$migrationCount = $migrationFiles.Count

# 4. Сервисы, реализующие ICompanyService
$serviceFiles = $csFiles | Select-String -Pattern "class\s+\w+\s*:\s*ICompanyService" -List
$serviceCount = $serviceFiles.Count

# 5. Контроллеры
$controllerFiles = $csFiles | Select-String -Pattern "class\s+\w+Controller\s*[:{]" -List
$controllerCount = $controllerFiles.Count

# 6. Сущности (атрибут [Table])
$entityFiles = $csFiles | Select-String -Pattern "\[Table\(" -List
$entityCount = $entityFiles.Count

# 7. Проекты (.csproj)
$projectFiles = Get-SourceFiles -Path $RootPath -Filter "*.csproj"
$projectCount = $projectFiles.Count

# 8. HTML/CSS
$htmlFiles = Get-SourceFiles -Path $RootPath -Filter "*.html"
$htmlCount = $htmlFiles.Count
$cssFiles = Get-SourceFiles -Path $RootPath -Filter "*.css"
$cssCount = $cssFiles.Count

# ---------- НОВЫЕ ПОКАЗАТЕЛИ ----------

# 9. Тесты (файлы с [Fact] или [Theory] или именем Tests.cs)
$testFiles = $csFiles | Where-Object {
    $_.Name -match "Tests\.cs$" -or
    (Select-String -Path $_.FullName -Pattern "\[Fact\]|\[Theory\]" -Quiet)
}
$testCount = $testFiles.Count
if ($testCount -gt 0) {
    $testLines = ($testFiles | ForEach-Object { (Get-Content $_.FullName | Measure-Object -Line).Lines } | Measure-Object -Sum).Sum
} else { $testLines = 0 }

# 10. Интерфейсы
$interfaceFiles = $csFiles | Select-String -Pattern "interface\s+I\w+" -List
$interfaceCount = $interfaceFiles.Count

# 11. Общие библиотеки (проекты с Contracts/Shared)
$sharedProjectFiles = $projectFiles | Where-Object { $_.Name -match "Contracts|Shared" }
$sharedProjectCount = $sharedProjectFiles.Count

# 12. DTO/модели (файлы с Dto, Request, Response в имени или в папке DTO)
$dtoFiles = $csFiles | Where-Object {
    $_.Name -match "Dto|Request|Response" -or $_.DirectoryName -match "DTO"
}
$dtoCount = $dtoFiles.Count

# 13. Методы в контроллерах (публичные действия)
$controllerMethodMatches = $controllerFiles | Select-String -Pattern "public\s+(async\s+)?(Task<.*?>|ActionResult|IActionResult)\s+\w+\(" -AllMatches
$controllerMethodCount = ($controllerMethodMatches | ForEach-Object { $_.Matches.Count } | Measure-Object -Sum).Sum

# 14. TODO/FIXME/HACK
$allSourceFiles = $csFiles + $jsFiles
$todoCount = ($allSourceFiles | Select-String -Pattern "TODO|FIXME|HACK" -AllMatches | Measure-Object).Count

# 15. NuGet-пакеты (уникальные)
$packageRefs = Get-ChildItem -Path $RootPath -Filter "*.csproj" -Recurse | ForEach-Object {
    Select-String -Path $_.FullName -Pattern '<PackageReference Include="([^"]+)"' -AllMatches
} | ForEach-Object { $_.Matches.Groups[1].Value } | Sort-Object -Unique
$packageCount = $packageRefs.Count

# 16. Git-статистика (если доступен)
$commitCount = "N/A"
$commitsLast30 = "N/A"
$authorCount = "N/A"
if (Get-Command git -ErrorAction SilentlyContinue) {
    try {
        $originalDir = Get-Location
        Set-Location $RootPath
        $commitCount = git rev-list --count HEAD 2>$null
        if (-not $commitCount) { $commitCount = "N/A" }

        $commitsLast30 = git log --since="30 days ago" --oneline 2>$null | Measure-Object | Select-Object -ExpandProperty Count
        if (-not $commitsLast30) { $commitsLast30 = "N/A" }

        $authorCount = git log --format='%aN' 2>$null | Sort-Object -Unique | Measure-Object | Select-Object -ExpandProperty Count
        if (-not $authorCount) { $authorCount = "N/A" }

        Set-Location $originalDir
    } catch {
        $commitCount = "N/A"
        $commitsLast30 = "N/A"
        $authorCount = "N/A"
    }
}

# 17. Средний размер файла C#
$avgSize = if ($csCount -gt 0) { [math]::Round($csLines / $csCount, 1) } else { 0 }

# 18. Покрытие тестами (приблизительное, по файлам)
$testCoverage = if ($csCount -gt 0) { [math]::Round(($testCount / $csCount) * 100, 1) } else { 0 }

# Формируем отчёт
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
  Сущностей ([Table]) : $entityCount
  Интерфейсов      : $interfaceCount
  DTO/моделей      : $dtoCount
  Методов в контроллерах : $controllerMethodCount
  Проектов (.csproj) : $projectCount
  Общие библиотеки (Contracts/Shared) : $sharedProjectCount

🧪 Тестирование:
  Тестовых файлов  : $testCount
  Строк тестов     : $testLines
  Покрытие (по файлам) : $testCoverage%

📦 Управление версиями:
  Коммитов (всего) : $commitCount
  Коммитов за 30 дней : $commitsLast30
  Уникальных авторов : $authorCount

🔧 Качество кода:
  TODO/FIXME/HACK  : $todoCount
  NuGet-пакетов    : $packageCount

=========================================
"@

# Вывод в консоль
Write-Host $report -ForegroundColor Green

# Сохранение в файл, если указан
if ($OutputFile) {
    $report | Out-File -FilePath $OutputFile -Encoding utf8
    Write-Host "✅ Отчёт сохранён в: $OutputFile" -ForegroundColor Yellow
}