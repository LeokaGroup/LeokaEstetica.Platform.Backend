using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce;

/// <summary>
/// Класс выходной модели создания заказа в кэше.
/// </summary>
public class CreateOrderCacheOutput : IFrontError
{
    /// <summary>
    /// PK.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// Процент скидки.
    /// </summary>
    public decimal Percent { get; set; }

    /// <summary>
    /// Сумма.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Месяц.
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// Id пользователя, которому принадлежит заказ.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }
}