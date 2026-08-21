# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2026-08-21

### Changed

#### API Versioning
- API version updated to **2.0.0**
- **VersionController** added to all microservices (Agro, Shipbuilding, School, NurseryHome) and MigrationWeb
  - Returns service name, assembly version, API version, and timestamp

#### Employee Professions Model
- **PrimaryProfession** DTO added — tracks primary profession with `Column`, `HireDate`, `FireDate`
- **CreateEmployeeRequest** refactored:
  - `AdditionalData` replaced with `Professions` (`Dictionary<string, bool>`)
  - Added `PrimaryProfession` property (`PrimaryProfession?`)
- **EmployeeAdditionalInfo** refactored:
  - `AdditionalData` replaced with `Professions` (`Dictionary<string, object>?`)
- **Shipbuilding**: Added `EmployeeProfession` entity with hire/fire date tracking
  - `AddEmployeeAsync` — creates EmployeeProfession record for primary profession on hire
  - `RemoveEmployeeAsync` — sets `FireDate` on current EmployeeProfession (saves hire history)
  - `UpdateEmployeeAsync` — handles profession change (fires old, hires new)
  - `GetProfessionsStatsAsync` — fixed date filter logic (`FireDate > DateTime.UtcNow` instead of `<`)

#### Frontend
- **Primary profession** displayed on employee detail page
- **Professions list** used instead of additionalData for employee creation form
- **Healthcheck** added to frontend dashboard
- **Icons** added for company pages
- **Dashboard.js** split into modular parts (`menu.js`, `navigator.js`, `renderers.js`, `utils.js`)
- **CSS** styles updated

### Fixed

- **MIG-69**: Logging refactoring — improved `LoggerExtensions`
- **MIG-58**: Fixed LINQ translation error — replaced `MatchFilter` with SQL-translatable expression in `GetFilteredEmployees`
- **MIG-58**: Shipbuilding employee count logic updated for new profession-based model
- **MIG-58**: Empty norms handled for School and NurseryHome companies

### Components

- **Migration.Contracts** v2.0.0
  - `ApiVersion` updated to 2.0.0
  - New `PrimaryProfession` DTO
  - Refactored `CreateEmployeeRequest` and `EmployeeAdditionalInfo` (Professions model)
  - VersionController endpoint in all microservices

- **Migration.Agro** v2.0.0
  - VersionController added
  - Professions-based hire/update logic

- **Migration.Shipbuilding** v2.0.0
  - New `EmployeeProfession` entity and table
  - Primary profession tracking on hire/update/remove
  - Enhanced resource forecast calculation with profession-based employee counts
  - VersionController added

- **Migration.School** v2.0.0
  - VersionController added

- **Migration.NurseryHome** v2.0.0
  - VersionController added

- **MigrationWeb** v2.0.0
  - Frontend: primary profession display, professions list in employee form
  - Frontend healthcheck integration
  - Company page icons
  - Dashboard JS refactored into modular files
  - Updated CSS styles

## [1.0.0] - 2026-07-14

### Added

#### Microservices
- **Migration.Contracts** — shared contracts, DTOs, EF migrations, middleware
  - ICompanyService interface
  - LoggerExtensions
  - CorrelationIdMiddleware + ErrorHandlingMiddleware
  - CorrelationIdHandler (DelegatingHandler)
  - ApplicationBuilderExtensions + ServiceCollectionExtensions
  - ServiceUrls, ServiceHealthStatus
  - CoreDBContext with 17 EF migrations
- **Migration.Agro** — HR service for agricultural company
  - HRServiceAgro
  - Docker container support
- **Migration.Shipbuilding** — HR service for shipbuilding company
  - HRServiceShipbuilding
  - Docker container support
- **Migration.School** — HR service for educational institutions
  - HRServiceSchool
  - Docker container support
- **Migration.NurseryHome** — HR service for nursery/home care
  - HRServiceNurseryHome
  - Docker container support

#### Core Database & Entities
- **Employee** entity with `FullName`, `BirthDate`, `CurrentCompany`, `IsDeleted`
- **Companies** table with `Name`, `Alias`, `Latitude`, `Longitude`, `Image`

#### DTOs (records)
- Employee: `CreateEmployeeRequest`, `RemoveEmployeeRequest`, `EmployeeAdditionalInfo`, `EmployeeSummaryInfo`, `EmployeeFilter`
- Companies: `Company`, `CompanyCountDTO`
- Professions: `ProfessionDTO`, `ProfessionCountDTO`, `ProfessionResourceNormDTO`
- Resources: `ResourceDTO`, `ResourceForecastDTO`

#### MigrationWeb
- **HRService** — centralized service orchestrating cross-company operations:
  - Employee CRUD across all company microservices
  - Employee transfer between companies (soft-delete old + add to new)
  - Company statistics with per-company breakdown + "All" row
- **CompanyService** — aggregates professions, resources, norms from all microservices
- **HTTPCompanyService** — REST client facade for all microservices
- **ServiceHealthChecker** — validates Core DB and all microservices at startup
- Keyed DI registration for `ICompanyService` (Agro, Shipbuilding, School, NurseryHome)
- CorrelationId middleware pipeline (`UseCorrelationId()`)
- Global error handling (`UseErrorHandling()`) with ProblemDetails JSON
- Swagger/OpenAPI documentation
- SPA Dashboard (index.html)

#### Correlation ID
- `CorrelationIdMiddleware` — extracts/generates `X-Correlation-ID`, attaches to log scope
- `CorrelationIdHandler` — propagates correlation ID to outgoing HTTP calls
- `AddCorrelationIdSupport()` / `UseCorrelationId()` extensions

#### Global Error Handling
- `ErrorHandlingMiddleware` — catches unhandled exceptions, logs with CorrelationId, returns JSON problem details

#### Tests
- **Migration.Agro.Tests** — xUnit + Moq + FluentAssertions for HRServiceAgro

### Components

- **Migration.Contracts** v1.0.0
  - ICompanyService interface (employees, professions, resources)
  - Employee DTOs (CreateEmployeeRequest, RemoveEmployeeRequest, EmployeeAdditionalInfo, EmployeeSummaryInfo, EmployeeFilter)
  - Company DTOs (Company, CompanyCountDTO)
  - Profession DTOs (ProfessionDTO, ProfessionCountDTO, ProfessionResourceNormDTO)
  - Resource DTOs (ResourceDTO, ResourceForecastDTO)
  - CoreDBContext + 17 EF migrations
  - ServiceUrls, ServiceHealthStatus
  - CorrelationIdMiddleware, ErrorHandlingMiddleware
  - LoggerExtensions, ApplicationBuilderExtensions, ServiceCollectionExtensions

- **Migration.Agro** v1.0.0
  - HRServiceAgro
  - Docker container support

- **Migration.Shipbuilding** v1.0.0
  - HRServiceShipbuilding
  - Docker container support

- **Migration.School** v1.0.0
  - HRServiceSchool
  - Docker container support

- **Migration.NurseryHome** v1.0.0
  - HRServiceNurseryHome
  - Docker container support

- **MigrationWeb** v1.0.0
  - HRService (centralized orchestrator)
  - CompanyService (aggregation)
  - HTTPCompanyService (HTTP clients)
  - ServiceHealthChecker
  - SPA Dashboard (index.html)
  - CorrelationId middleware pipeline
  - Global error handling middleware
  - Swagger/OpenAPI documentation
