using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;

/// <summary>
/// Класс выходной модели управления комментарием проекта на модерации.
/// </summary>
public class ManagingProjectCommentModerationOutput : IFrontError
{
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }

    /// <summary>
    /// Признак успеха.
    /// </summary>
    public bool IsSuccess { get; set; }
}