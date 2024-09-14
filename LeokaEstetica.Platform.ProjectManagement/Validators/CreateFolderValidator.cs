using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора создания папки Wiki проекта.
/// </summary>
public class CreateFolderValidator : AbstractValidator<CreateWikiFolderInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public CreateFolderValidator()
    {
        RuleFor(p => p.FolderName)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_CURRENT_FOLDER_NAME)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_CURRENT_FOLDER_NAME);
        
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);
    }
}