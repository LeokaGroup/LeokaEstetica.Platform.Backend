using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Commerce;

/// <summary>
/// Класс выходной модели создания заказа в кэше.
/// </summary>
public class CreateOrderCacheOutput : IFrontError
{
    /// <summary>
    /// Название тарифа.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Стоимость тарифа (с учетом всех скидок, сервисов, акций).
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }
}