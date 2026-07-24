---
mode: all
apply: apply
---

# Project Context: .NET Microservices (Backend)

## General Rules
- All new DTOs must be declared as `record`, not `class`. They should use `{ get; init; }`.
- Example: `public record EmployeeFilter(string? Company, string? Profession);`
