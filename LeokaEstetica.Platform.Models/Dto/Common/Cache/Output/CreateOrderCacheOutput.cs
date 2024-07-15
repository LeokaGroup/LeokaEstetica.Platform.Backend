using FluentValidation.Results;

namespace LeokaEstetica.Platform.Models.Dto.Common.Cache.Output;

/// <summary>
/// Класс выходной модели создания заказа в кэше.
/// </summary>
public class CreateOrderCacheOutput : IFrontError
{
    /// <summary>
    /// Id тарифа.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// Сумма.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Месяц.
    /// </summary>
    public short Month { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Название тарифа.
    /// </summary>
    public string? FareRuleName { get; set; }

    /// <summary>
    /// Признак успешности прохождения по лимитам.
    /// </summary>
    public bool IsSuccessLimits { get; set; }

    /// <summary>
    /// Тип лимитов, по которым не прошли.
    /// </summary>
    public string? ReductionSubscriptionLimits { get; set; }

    /// <summary>
    /// Кол-во на которое нужно уменьшить для прохождения по лимитам.
    /// </summary>
    public int FareLimitsCount { get; set; }

    /// <summary>
    /// Ошибки, если они есть.
    /// </summary>
    public List<ValidationFailure>? Errors { get; set; }

    /// <summary>
    /// Признак необходимости ожидания действий пользователя.
    /// </summary>
    public bool IsNeedUserAction { get; set; }
}