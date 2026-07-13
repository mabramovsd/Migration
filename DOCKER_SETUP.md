# Docker Setup for Migration Project

## Что было сделано

1. **Docker Compose настроен** - в корне проекта создан `docker-compose.yml` с двумя сервисами:
   - `mssql` - MS SQL Server 2022
   - `webapi` - ASP.NET Core Web API

2. **Dockerfile для Web API** - находится в `MigrationWeb/Dockerfile` и собирает приложение с использованием multi-stage build.

3. **Базы данных** - автоматически создаются при первом запуске контейнера API.

## Как запустить

```bash
# Запуск всех сервисов
docker-compose up -d

# Просмотр логов
docker logs petproject_api
docker logs petproject_mssql

# Остановка
docker-compose down
```

## Перестроение
```bash
docker-compose build webapi
```

## Порты

- **SQL Server**: 1433
- **Web API**: 8080 (HTTP), 8081 (HTTPS)

## Connection Strings

Для подключения из контейнера используйте:
```
Server=mssql,1433;Database=Migration_Agro;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True
```

Для подключения с хоста используйте:
```
Server=localhost,1433;Database=Migration_Agro;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True
```


## Дополнительные файлы

- `.dockerignore` - исключает ненужные файлы из образа
- `docker-compose.override.yml` - переопределяет настройки для разработки
- `apply-migrations.sh` - скрипт для ручного применения миграций (если нужно)

```
# Run migrations using the Web API project
dotnet ef database update --connection "Server=mssql,1433;Database=Migration_Agro;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True" --project MigrationWeb/MigrationWeb.csproj --startup-project MigrationWeb/MigrationWeb.csproj
dotnet ef database update --connection "Server=mssql,1433;Database=Migration_Shipbuilding;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True" --project MigrationWeb/MigrationWeb.csproj --startup-project MigrationWeb/MigrationWeb.csproj
dotnet ef database update --connection "Server=mssql,1433;Database=Migration_Core;User Id=SA;Password=Your_strong_P@ssw0rd123;Trust Server Certificate=True" --project MigrationWeb/MigrationWeb.csproj --startup-project MigrationWeb/MigrationWeb.csproj
```
