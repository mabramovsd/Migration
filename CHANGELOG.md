# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
