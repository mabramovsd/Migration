using FluentValidation;
using Migration.Contracts.DTO.Employees;

namespace Migration.Contracts.Validators;

public class RemoveEmployeeRequestValidator : AbstractValidator<RemoveEmployeeRequest>
{
    public RemoveEmployeeRequestValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty().WithMessage("Employee ID is required");
    }
}
