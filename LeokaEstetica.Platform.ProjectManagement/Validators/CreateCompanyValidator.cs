using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора создания компании.
/// </summary>
public class CreateCompanyValidator : AbstractValidator<CompanyInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CreateCompanyValidator()
    {
        RuleFor(p => p.CompanyName)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_COMPANY_NAME)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_COMPANY_NAME);
    }
}