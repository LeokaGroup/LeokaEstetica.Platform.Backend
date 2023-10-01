using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Ticket;

/// <summary>
/// Класс выходной модели для создания предложения/пожелания.
/// </summary>
public class WisheOfferOutput : IFrontError
{
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }

    /// <summary>
    /// Признак успешности.
    /// </summary>
    public bool IsSuccess { get; set; }
}