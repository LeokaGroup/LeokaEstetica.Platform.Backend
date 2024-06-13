using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора изменения названия папки.
/// </summary>
public class ChangeFolderNameValidator : AbstractValidator<UpdateFolderNameInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ChangeFolderNameValidator()
    {
        RuleFor(p => p.FolderName)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_CURRENT_FOLDER_NAME)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_CURRENT_FOLDER_NAME);

        RuleFor(p => p.FolderId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_FOLDER_ID);
    }
}