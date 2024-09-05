using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

namespace LeokaEstetica.Platform.ProjectManagement.Validators;

/// <summary>
/// Класс валидатора ролей пользователя модуля УП.
/// </summary>
public class ProjectManagementRoleValidator : AbstractValidator<UpdateRoleInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ProjectManagementRoleValidator()
    {
        RuleFor(p => p)
            .Must(p => p.Roles is not null && p.Roles.All(x => x.RoleId > 0))
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_ROLE_ID);

        RuleFor(p => p)
            .Must(p => p.Roles is not null && p.Roles.All(x => x.UserId > 0))
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_USER_ID);

        RuleFor(p => p)
            .Must(p => p.ProjectId > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);

        RuleFor(p => p)
            .Must(p => p.CompanyId > 0)
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_COMPANY_ID);
    }
}