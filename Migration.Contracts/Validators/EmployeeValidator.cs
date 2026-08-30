using FluentValidation;
using Migration.Contracts.DTO.Employees;

namespace Migration.Contracts.Validators;

public class EmployeeValidator : AbstractValidator<Employee>
{
    public EmployeeValidator()
    {
        RuleFor(e => e.FullName)
            .NotEmpty().WithMessage("Full Name is required")
            .Length(2, 200).WithMessage("Full Name must be between 2 and 200 characters");

        RuleFor(e => e.BirthDate)
            .LessThan(DateTime.Now.AddYears(-13)).WithMessage("Employee must be at least 14 years old")
            .GreaterThanOrEqualTo(DateTime.Now.AddYears(-150)).WithMessage("Birth date cannot be more than 150 years ago");

        RuleFor(e => e.CurrentCompany)
            .MaximumLength(50).WithMessage("Current Company cannot exceed 50 characters");
    }
}
