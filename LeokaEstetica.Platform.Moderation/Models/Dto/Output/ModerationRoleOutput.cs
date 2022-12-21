using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Moderation.Models.Dto.Output;

/// <summary>
/// Класс выходной модели ролей модерации.
/// </summary>
public class ModerationRoleOutput : IFrontError
{
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }
}