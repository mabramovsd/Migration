using FluentValidation;
using Migration.Contracts.DTO.Employees;

namespace Migration.Contracts.Validators;

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(r => r.CoreData)
            .SetValidator(new EmployeeValidator());
    }
}
