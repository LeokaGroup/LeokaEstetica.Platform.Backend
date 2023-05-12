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
    /// Сумма скидки.
    /// </summary>
    [ProtoMember(3)]
    public decimal? Price { get; set; }

    /// <summary>
    /// Месяц.
    /// </summary>
    [ProtoMember(4)]
    public int Month { get; set; }
}