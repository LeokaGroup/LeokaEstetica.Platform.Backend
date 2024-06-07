using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора ролей пользователя модуля УП.
/// </summary>
public class ProjectManagementRoleValidator : AbstractValidator<IEnumerable<ProjectManagementRoleInput>>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ProjectManagementRoleValidator()
    {
        RuleFor(p => p)
            .Must(p => p.All(x => x.RoleId > 0))
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_ROLE_ID);
            
        RuleFor(p => p)
            .Must(p => p.All(x => x.UserId > 0))
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_USER_ID);
    }
}