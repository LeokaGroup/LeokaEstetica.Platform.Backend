using FluentValidation.Results;

namespace LeokaEstetica.Platform.Models.Dto.Common;

/// <summary>
/// Абстракция описывающая ошибки.
/// </summary>
public interface IFrontError
{
    /// <summary>
    /// Список ошибок, которые будем выводить на фронт.
    /// </summary>
    List<ValidationFailure> Errors { get; set; }
}