using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.Search.Project;

namespace LeokaEstetica.Platform.Controllers.Validators.Search;

/// <summary>
/// Класс валидатора приглашений пользователей в команду проекта.
/// </summary>
public class SearchInviteProjectMembersValidator : AbstractValidator<SearchProjectMemberInput>
{
    public SearchInviteProjectMembersValidator()
    {
        RuleFor(p => p.SearchText)
            .NotEmpty()
            .WithMessage(GlobalConfigKeys.SearchProject.NOT_EMPTY_SEARCH_TEXT)
            .NotNull()
            .WithMessage(GlobalConfigKeys.SearchProject.NOT_EMPTY_SEARCH_TEXT)
            .MaximumLength(100)
            .WithMessage(GlobalConfigKeys.SearchProject.MAX_LENGHT_EXCEEDED);
    }
}