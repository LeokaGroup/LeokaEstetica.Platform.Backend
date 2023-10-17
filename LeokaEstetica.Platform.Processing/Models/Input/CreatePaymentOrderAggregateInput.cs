using LeokaEstetica.Platform.Models.Dto.Common.Cache;
using LeokaEstetica.Platform.Models.Dto.Input.Base;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;

namespace LeokaEstetica.Platform.Processing.Models.Input;

/// <summary>
/// Класс входной модели агрегирующей модели со всеми нужными полями для создания заказа.
/// </summary>
public class CreatePaymentOrderAggregateInput
{
    /// <summary>
    /// Даные выходной модели заказа.
    /// </summary>
    public ICreateOrderOutput CreateOrderOutput { get; set; }

    /// <summary>
    /// Данные заказа из кэша.
    /// </summary>
    public CreateOrderCache OrderCache { get; set; }

    /// <summary>
    /// Даные входной модели заказа.
    /// </summary>
    public ICreateOrderInput CreateOrderInput { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Публичный ключ тарифа.
    /// </summary>
    public Guid PublicId { get; set; }

    /// <summary>
    /// Название тарифа.
    /// </summary>
    public string FareRuleName { get; set; }

    /// <summary>
    /// Аккаунт пользователя.
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// Кол-во месяцев, на которое оформлен тариф.
    /// </summary>
    public short Month { get; set; }
}