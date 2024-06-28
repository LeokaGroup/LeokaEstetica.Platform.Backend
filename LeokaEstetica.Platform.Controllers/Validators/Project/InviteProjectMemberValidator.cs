using FluentValidation;
using LeokaEstetica.Platform.Controllers.ModelsValidation.Project;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectTeam;
using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Controllers.Validators.Project;

/// <summary>
/// Класс валидатора добавления в команду проекта пользователя.
/// </summary>
public class InviteProjectMemberValidator : AbstractValidator<InviteProjectMemberInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public InviteProjectMemberValidator()
    {
        RuleFor(p => p.InviteText)
            .NotEmpty()
            .NotNull()
            .WithMessage(ValidationConsts.NOT_VALID_INVITE_PROJECT_TEAM_USER);
        
        RuleFor(p => Enum.Parse<ProjectInviteTypeEnum>(p.InviteType))
            .Must(p=> Enum.IsDefined(typeof(ProjectInviteTypeEnum), p))
            .WithMessage(ValidationConsts.NOT_VALID_INVITE_TYPE);
        
        RuleFor(p => p.ProjectId)
            .GreaterThan(0)
            .WithMessage(ValidationConsts.NOT_VALID_INVITE_PROJECT_TEAM_PROJECT_ID);
        
        RuleFor(p => p.VacancyId)
            .GreaterThan(0)
            .WithMessage(ValidationConsts.NOT_VALID_INVITE_PROJECT_TEAM_VACANCY_ID);
    }

}