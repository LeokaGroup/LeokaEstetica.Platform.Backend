using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора создания страницы Wiki проекта.
/// </summary>
public class CreatePageValidator : AbstractValidator<CreateWikiPageInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CreatePageValidator()
    {
        RuleFor(p => p.PageName)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PAGE_NAME)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PAGE_NAME);

        RuleFor(p => p.WikiTreeId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_WIKI_TREE_ID);
    }
}