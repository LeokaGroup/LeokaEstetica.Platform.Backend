namespace LeokaEstetica.Platform.Models.Entities.FareRule;

/// <summary>
/// Класс сопоставляется с таблицей элементов тарифов Rules.FareRulesItems.
/// </summary>
public class FareRuleItemEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int RuleItemId { get; set; }

    /// <summary>
    /// Id правила тарифа.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// FK.
    /// </summary>
    public FareRuleEntity FareRule { get; set; }

    /// <summary>
    /// Название пункта тарифа.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Метка тарифа.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Признак указывает на то, что функционал будет позже. Отображается метка "Скоро".
    /// </summary>
    public bool IsLater { get; set; }

    /// <summary>
    /// Позиция элемента в списке.
    /// </summary>
    public int Position { get; set; }
}