using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора получения структуры папки.
/// </summary>
public class GetTreeItemFolderValidator : AbstractValidator<(long ProjectId, long FolderId)>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public GetTreeItemFolderValidator()
    {
        RuleFor(p => p.ProjectId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);

        RuleFor(p => p.FolderId)
            .Must(p => p > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_FOLDER_ID);
    }
}