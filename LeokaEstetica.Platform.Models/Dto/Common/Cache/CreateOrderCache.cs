using ProtoBuf;

namespace LeokaEstetica.Platform.Models.Dto.Common.Cache;

/// <summary>
/// Класс модели для хранения заказа в кэше.
/// </summary>
[Serializable]
[ProtoContract]
public class CreateOrderCache
{
    /// <summary>
    /// PK.
    /// </summary>
    [ProtoMember(1)]
    public int RuleId { get; set; }

    /// <summary>
    /// Процент скидки.
    /// </summary>
    [ProtoMember(2)]
    public decimal Percent { get; set; }

    /// <summary>
    /// Сумма.
    /// </summary>
    [ProtoMember(3)]
    public decimal Price { get; set; }

    /// <summary>
    /// Месяц.
    /// </summary>
    [ProtoMember(4)]
    public short Month { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    [ProtoMember(5)]
    public long UserId { get; set; }

    /// <summary>
    /// Список сервисов для услуг.
    /// </summary>
    [ProtoMember(6)]
    public List<string> Products { get; set; }
}