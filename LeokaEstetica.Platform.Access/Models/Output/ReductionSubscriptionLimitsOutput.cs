using FluentValidation.Results;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Access.Models.Output;

/// <summary>
/// Класс выходной модели лимитов подписки.
/// </summary>
public class ReductionSubscriptionLimitsOutput : IFrontError
{
    /// <summary>
    /// Признак успешности прохождения по лимитам.
    /// </summary>
    public bool IsSuccessLimits { get; set; }

    /// <summary>
    /// Тип лимитов, по которым не прошли.
    /// </summary>
    public ReductionSubscriptionLimitsEnum ReductionSubscriptionLimits { get; set; }

    /// <summary>
    /// Кол-во на которое нужно уменьшить для прохождения по лимитам.
    /// </summary>
    public int FareLimitsCount { get; set; }
    
    /// <summary>
    /// Ошибки, если они есть.
    /// </summary>
    public List<ValidationFailure>? Errors { get; set; }
}