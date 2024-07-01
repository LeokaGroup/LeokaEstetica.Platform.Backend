using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора изменения названия страницы папки.
/// </summary>
public class ChangePageFolderNameValidator : AbstractValidator<UpdateFolderPageNameInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ChangePageFolderNameValidator()
    {
        RuleFor(p => p.PageName)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PAGE_NAME)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PAGE_NAME);

        RuleFor(p => p.PageId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PAGE_ID);
    }
}