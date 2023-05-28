namespace LeokaEstetica.Platform.Models.Entities.Configs;

/// <summary>
/// Класс сопоставляется с управляющей таблицей исключения полей при валидации Configs.ValidationColumnsExclude.
/// </summary>
public class ValidationColumnExcludeEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int ValidationId { get; set; }

    /// <summary>
    /// Название параметра, который нужно исключать при валидации.
    /// </summary>
    public string ParamName { get; set; }
}