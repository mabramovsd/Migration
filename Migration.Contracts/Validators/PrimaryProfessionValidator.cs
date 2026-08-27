using FluentValidation;
using Migration.Contracts.DTO.Professions;

namespace Migration.Contracts.Validators;

public class PrimaryProfessionValidator : AbstractValidator<PrimaryProfession>
{
    public PrimaryProfessionValidator()
    {
        RuleFor(p => p.Column)
            .NotEmpty().WithMessage("Column is required")
            .MaximumLength(50).WithMessage("Column cannot exceed 50 characters");

        RuleFor(p => p.HireDate)
            .LessThanOrEqualTo(DateTime.Now.AddMinutes(1))
            .WithMessage("Hire date cannot be in the future");
    }
}
