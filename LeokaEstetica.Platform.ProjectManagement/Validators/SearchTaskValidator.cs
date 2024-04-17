using FluentValidation;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Models.Dto.Input.Search.ProjectManagment;

namespace LeokaEstetica.Platform.ProjectManagment.Validators;

/// <summary>
/// Класс валидатора поиска задач.
/// </summary>
public class SearchTaskValidator : AbstractValidator<SearchTaskInput>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public SearchTaskValidator()
    {
        RuleFor(p => p.ProjectIds)
            .Must(p => p.Any())
            .WithMessage(ValidationConst.ProjectManagmentValidation.NOT_VALID_PROJECT_ID);

        RuleFor(p => p.SearchText)
            .NotNull()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_SEARCH_TEXT)
            .NotEmpty()
            .WithMessage(ValidationConst.ProjectManagmentValidation.EMPTY_SEARCH_TEXT);
    }
}