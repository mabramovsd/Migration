---
mode: all
apply: apply
---

# Project Context: .NET Microservices (Backend)

## General Rules
- All new DTOs must be declared as `record`, not `class`. They should use `{ get; init; }`.
- Example: `public record EmployeeFilter(string? Company, string? Profession);`

## Entity Framework Core Migrations

1.  **Base Class:** Always use the fully qualified name for the base class to avoid namespace conflicts.
    *   **Correct:** `public partial class AddResourcesTable : Microsoft.EntityFrameworkCore.Migrations.Migration`
    *   **Incorrect:** `public partial class AddResourcesTable : Migration` (Causes ambiguity if a local 'Migration' class exists).
    
2.  **File Naming:** The migration file name must be prefixed with a timestamp and match the class name exactly.
    *   Format: `yyyyMMddHHmmss_ClassName.cs`
    *   Example: `20260724120000_AddResourcesTable.cs`

3.  **Constructor:** Do not override the parameterless constructor unless you are injecting dependencies manually. Let EF Core handle it.