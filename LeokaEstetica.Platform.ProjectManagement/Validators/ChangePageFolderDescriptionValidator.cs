using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора изменения описания страницы папки.
/// </summary>
public class ChangePageFolderDescriptionValidator : AbstractValidator<UpdateFolderPageDescriptionInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ChangePageFolderDescriptionValidator()
    {
        RuleFor(p => p.PageDescription)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PAGE_DESCRIPTION)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PAGE_DESCRIPTION);

        RuleFor(p => p.PageId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PAGE_ID);
    }
}