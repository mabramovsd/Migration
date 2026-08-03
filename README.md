# Migration System

Система управления персоналом для демонстрации микросервисной архитектуры, миграций Entity Framework Core и REST API композиции.

## 📄 Содержание
- [💡 О проекте](#Описание)
- [🏗️ Архитектура](#Архитектура)
- [🛠️ Технологии](#Технологии)
- [🚀 Запуск проекта](#Запуск)
- [🎮 Как это работает?](#Инструкция)
- [📈 Что я получу от этого проекта?](#Зачем)
- [📑 Структура решения](#Структура)
- [🔧 Настройка Docker](#Docker-настройка)

# Описание

Система управления персоналом для нескольких компаний (Сельское хозяйство, Судостроительство, Школа, Детский сад). Проект демонстрирует:
- Микросервисную архитектуру без RabbitMQ (прямое HTTP-взаимодействие)
- REST API композицию через центральный API Gateway
- Разделение данных между сервисами
- Shared Kernel паттерн (общие DTO и интерфейсы)
- SPA-фронтенд с дашбордом и фильтрацией сотрудников
- Утилиту для межбазовых миграций данных

# Архитектура

Проект построен на принципах микросервисной архитектуры:

- **Разделение ответственности** — каждый бизнес-аспект в отдельном сервисе (Agro, Shipbuilding, School, NurseryHome)
- **Независимость данных** — у каждого сервиса своя база данных
- **REST API композиция** — MigrationWeb агрегирует данные с всех сервисов
- **Shared Kernel** — общие DTO и интерфейсы в Migration.Contracts
- **HTTP-взаимодействие** — сервисы общаются напрямую через HTTP (без RabbitMQ)
- **Migration Tools** — утилита для миграции данных между базами

# Технологии

- Язык и фреймворк: C#, .NET 8, ASP.NET Core Web API
- Клиентская часть: SPA-приложение (Vanilla JS)
- СУБД: MS SQL Server (одна БД с несколькими схемами)
- ORM: Entity Framework Core (миграции)
- Инъекция зависимостей: Встроенный DI-контейнер .NET
- Docker: Поддержка контейнеризации всех сервисов и БД

# Запуск

## Docker-запуск (рекомендуется)

```bash
# Запуск всех сервисов
docker-compose up -d

# Просмотр логов
docker logs petproject_api
docker logs petproject_agro
docker logs petproject_shipbuilding
docker logs petproject_school
docker logs petproject_nurseryhome

# Остановка
docker-compose down
```

## Локальный запуск (без Docker)

**Предварительные требования:**
- MS SQL Server (LocalDB или Developer Edition)
- .NET SDK версии 8.0 или выше

**Шаги:**

1. Запустите MS SQL Server

2. Примените миграции (через MigrationWeb):
```bash
# Применить миграции для всех баз данных
dotnet ef database update --connection "Server=localhost;Database=Migration_Core;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True" --project MigrationWeb/MigrationWeb.csproj --startup-project MigrationWeb/MigrationWeb.csproj
dotnet ef database update --connection "Server=localhost;Database=Migration_Agro;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True" --project MigrationWeb/MigrationWeb.csproj --startup-project MigrationWeb/MigrationWeb.csproj
dotnet ef database update --connection "Server=localhost;Database=Migration_Shipbuilding;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True" --project MigrationWeb/MigrationWeb.csproj --startup-project MigrationWeb/MigrationWeb.csproj
dotnet ef database update --connection "Server=localhost;Database=Migration_School;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True" --project MigrationWeb/MigrationWeb.csproj --startup-project MigrationWeb/MigrationWeb.csproj
dotnet ef database update --connection "Server=localhost;Database=Migration_NurseryHome;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True" --project MigrationWeb/MigrationWeb.csproj --startup-project MigrationWeb/MigrationWeb.csproj
```

3. Запустите сервисы (в разных терминалах):
```bash
# Терминал 1: Agro Service
cd Migration.Agro
dotnet run

# Терминал 2: Shipbuilding Service
cd Migration.Shipbuilding
dotnet run

# Терминал 3: School Service
cd Migration.School
dotnet run

# Терминал 4: NurseryHome Service
cd Migration.NurseryHome
dotnet run

# Терминал 5: Web API
cd MigrationWeb
dotnet run
```

4. Дашборд будет доступен по адресу http://localhost:8080

# Инструкция

Все действия в системе инициируются через MigrationWeb (API Gateway).

**Поток взаимодействия:**
1. Вы совершаете действие на фронте (например, "Просмотреть сотрудников компании")
2. Фронтенд отправляет HTTP-запрос на соответствующий контроллер в MigrationWeb
3. Контроллер вызывает нужный сервис через HTTP-клиент
4. Данные агрегируются и отображаются на дашборде

**Доступные операции через дашборд:**
- Просмотр количества сотрудников по компаниям
- Просмотр статистики по профессиям внутри компании
- Просмотр списка сотрудников по фильтрам (компания, профессия)

# Зачем

- **Микросервисы без сложности** — увидьте архитектуру без RabbitMQ и Event-Driven
- **REST API композиция** — практика агрегации данных с нескольких сервисов
- **EF Core миграции** — безопасно экспериментируйте с схемой данных
- **Shared Kernel** — как поддерживать общие контракты между сервисами
- **Портфолио** — демонстрационный проект для собеседований

# Структура

```
Migration.sln
├── Migration.Contracts/          # Shared Kernel (DTO, Interfaces)
│   ├── DTO/
│   │   ├── Employees/           # Employee DTOs
│   │   ├── Companies/           # Company-related DTOs
│   │   └── Professions/         # Profession DTOs
│   ├── ICompanyService.cs       # Общий интерфейс сервисов
│   └── ServiceUrls.cs           # Настройки URL сервисов
├── Migration.Agro/              # Agro Service (Сельское хозяйство)
│   ├── Controllers/HRController.cs
│   ├── Services/HRServiceAgro.cs
│   ├── Entities/EmployeeAgro.cs
│   └── AgroDBContext.cs
├── Migration.Shipbuilding/      # Shipbuilding Service (Судостроительство)
│   ├── Controllers/HRController.cs
│   ├── Services/HRServiceShipbuilding.cs
│   ├── Entities/EmployeeShipbuilding.cs
│   └── ShipbuildingDBContext.cs
├── Migration.School/            # School Service (Школа)
│   ├── Controllers/HRController.cs
│   ├── Services/HRServiceSchool.cs
│   ├── Entities/EmployeeSchool.cs
│   └── SchoolDBContext.cs
├── Migration.NurseryHome/       # NurseryHome Service (Детский сад)
│   ├── Controllers/HRController.cs
│   ├── Services/HRServiceNurseryHome.cs
│   ├── Entities/EmployeeNurseryHome.cs
│   └── NurseryHomeDBContext.cs
├── Migration.MigrationTools/    # Утилита для миграции данных между БД
│   ├── Program.cs
│   └── appsettings.json
└── MigrationWeb/                # API Gateway + SPA
    ├── Controllers/HRController.cs
    ├── Services/
    │   ├── HTTPCompanyService.cs  # HTTP-клиент для сервисов
    │   └── HRService.cs           # Агрегация данных
    ├── wwwroot/                 # SPA-приложение
    │   ├── index.html
    │   ├── css/styles.css
    │   └── js/
    │       ├── dashboard.js
    │       ├── companies.js
    │       ├── menu.js
    │       ├── utils.js
    │       └── dashboard/
    │           ├── menu.js
    │           ├── navigator.js
    │           └── renderers.js
    └── appsettings.json         # Конфигурация (ServiceUrls)
```

# Docker-настройка

## Порты

- **SQL Server**: 1433
- **Agro Service**: 5002
- **Shipbuilding Service**: 5001
- **School Service**: 5003
- **NurseryHome Service**: 5004
- **Web API**: 8080 (HTTP), 8081 (HTTPS)

## Connection Strings

**Из контейнеров:**
```
Server=mssql,1433;Database=Migration_Core;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True
Server=mssql,1433;Database=Migration_Agro;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True
Server=mssql,1433;Database=Migration_Shipbuilding;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True
Server=mssql,1433;Database=Migration_School;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True
Server=mssql,1433;Database=Migration_NurseryHome;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True
```

**С хоста:**
```
Server=localhost,1433;Database=Migration_Core;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True
```

## Команды Docker

```bash
# Запуск всех сервисов
docker-compose up -d

# Просмотр логов
docker logs petproject_api
docker logs petproject_agro
docker logs petproject_shipbuilding
docker logs petproject_school
docker logs petproject_nurseryhome

# Остановка
docker-compose down

# Перестроение конкретного сервиса
docker-compose build agro
docker-compose build shipbuilding
docker-compose build school
docker-compose build nurseryhome
docker-compose build webapi
```

## Миграции в Docker

Миграции применяются автоматически при первом запуске API в Development-режиме.

Для ручного применения:
```bash
dotnet ef database update --connection "Server=mssql,1433;Database=Migration_Core;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True" --project MigrationWeb/MigrationWeb.csproj --startup-project MigrationWeb/MigrationWeb.csproj
```
