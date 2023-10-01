namespace LeokaEstetica.Platform.Models.Dto.Output.FareRule;

/// <summary>
/// Класс выходной модели правил тарифов.
/// </summary>
public class FareRuleOutput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа для отображения в строке URL.</param>
    public FareRuleOutput(Guid publicId)
    {
        PublicId = publicId;
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
    /// Публичный код для отображения в строке URL.
    /// </summary>
    public Guid PublicId { get; }

    /// <summary>
    /// Список элементов правил тарифов.
    /// </summary>
    public IEnumerable<FareRuleItemOutput> FareRuleItems { get; set; }
}