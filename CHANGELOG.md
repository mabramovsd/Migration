# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-07-14

### Added
- Initial release of Migration System
- Employee management API (HRController)
- HR Service for Agro and Shipbuilding companies
- Category statistics endpoint (`/HR/Stats/CategoryCounts`)
- SPA Dashboard with employee overview
- Docker support for all services

### Components
- **Migration.Contracts** v1.0.0
  - ICompanyService interface
  - Employee DTOs (Employee, EmployeeAdditionalInfo, EmployeeSummaryInfo)
  - Request DTOs (CreateEmployeeRequest, RemoveEmployeeRequest)
  - CategoryCountDTO for statistics
  
- **Migration.Agro** v1.0.0
  - HRController implementation
  - HRServiceAgro for Agro company
  - Docker container support
  
- **Migration.Shipbuilding** v1.0.0
  - HRController implementation
  - HRServiceShipbuilding for Shipbuilding company
  - Docker container support
  
- **MigrationWeb** v1.0.0
  - HRController with versioned endpoints
  - SPA Dashboard (index.html)
  - HTTP client for company services
  - Swagger/OpenAPI documentation

### Breaking Changes
None

### Deprecations
None
