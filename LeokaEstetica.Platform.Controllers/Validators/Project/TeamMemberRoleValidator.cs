using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectTeam;

namespace LeokaEstetica.Platform.Controllers.Validators.Project;

/// <summary>
/// Валидатор роли участника команды проекта.
/// </summary>
public class TeamMemberRoleValidator : AbstractValidator<TeamMemberRoleInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public TeamMemberRoleValidator()
    {
        RuleFor(p => p.Role)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectValidation.EMPTY_ROLE)
            .NotNull()
            .WithMessage(ValidationConst.ProjectValidation.EMPTY_ROLE);

        RuleFor(p => p.ProjectId)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectValidation.NOT_VALID_PROJECT_ID)
            .NotNull()
            .WithMessage(ValidationConst.ProjectValidation.NOT_VALID_PROJECT_ID);
            
        RuleFor(p => p.UserId)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectValidation.NOT_VALID_USER_ID)
            .NotNull()
            .WithMessage(ValidationConst.ProjectValidation.NOT_VALID_USER_ID);
    }
}