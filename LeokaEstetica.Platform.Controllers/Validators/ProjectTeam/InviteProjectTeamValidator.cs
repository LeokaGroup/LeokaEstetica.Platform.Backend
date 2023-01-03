using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectTeam;

namespace LeokaEstetica.Platform.Controllers.Validators.ProjectTeam;

/// <summary>
/// Класс валидатора добавления участников команды проекта.
/// </summary>
public class InviteProjectTeamValidator : AbstractValidator<InviteProjectMemberInput>
{
    public InviteProjectTeamValidator()
    {
        RuleFor(p => p.UsersIds)
            .NotEmpty()
            .WithMessage(GlobalConfigKeys.ProjectValidation.EMPTY_PROJECT_NAME)
            .NotNull()
            .WithMessage(GlobalConfigKeys.ProjectValidation.EMPTY_PROJECT_NAME);
    }
}