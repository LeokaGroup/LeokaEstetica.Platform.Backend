using ProtoBuf;

namespace LeokaEstetica.Platform.Models.Dto.Common.Cache;

/// <summary>
/// Класс модели для хранения заказа в кэше.
/// </summary>
[ProtoContract]
public class CreateOrderCache
{
    /// <summary>
    /// Id тарифа.
    /// </summary>
    [ProtoMember(1)]
    public int RuleId { get; set; }

    /// <summary>
    /// Сумма.
    /// </summary>
    [ProtoMember(2)]
    public decimal Price { get; set; }

    /// <summary>
    /// Месяц.
    /// </summary>
    [ProtoMember(3)]
    public short Month { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    [ProtoMember(4)]
    public long UserId { get; set; }

    /// <summary>
    /// Название тарифа.
    /// </summary>
    [ProtoMember(5)]
    public string? FareRuleName { get; set; }

    /// <summary>
    /// Признак успешности прохождения по лимитам.
    /// </summary>
    [ProtoIgnore]
    public bool IsSuccessLimits { get; set; }

    /// <summary>
    /// Тип лимитов, по которым не прошли.
    /// </summary>
    [ProtoIgnore]
    public string? ReductionSubscriptionLimits { get; set; }
}