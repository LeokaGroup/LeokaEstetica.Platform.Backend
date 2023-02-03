namespace LeokaEstetica.Platform.Models.Entities.FareRule;

/// <summary>
/// Класс сопоставляется с таблицей правил тарифа Rules.FareRules.
/// </summary>
public class FareRuleEntity
{
    public FareRuleEntity()
    {
        FareRuleItems = new HashSet<FareRuleItemEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// Название тарифа.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Позиция в списке.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Пометка к тарифу.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Стоимость тарифа.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Валюта тарифа.
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Признак популярного тарифа.
    /// </summary>
    public bool IsPopular { get; set; }

    /// <summary>
    /// Признак бесплатного тарифа.
    /// </summary>
    public bool IsFree { get; set; }

    /// <summary>
    /// Список элементов тарифов.
    /// </summary>
    public ICollection<FareRuleItemEntity> FareRuleItems { get; set; }
}