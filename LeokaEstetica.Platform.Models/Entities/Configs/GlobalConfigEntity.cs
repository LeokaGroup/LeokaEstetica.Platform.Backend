namespace LeokaEstetica.Platform.Models.Entities.Configs;

/// <summary>
/// Класс сопоставляется с таблицей глобальный ключей Configs.GlobalConfig.
/// </summary>
public class GlobalConfigEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ParamId { get; set; }

    /// <summary>
    /// Ключ.
    /// </summary>
    public string ParamKey { get; set; }

    /// <summary>
    /// Значение.
    /// </summary>
    public string ParamValue { get; set; }

    /// <summary>
    /// Тип данных значения. Например, Integer, Decimal, Boolean.
    /// </summary>
    public string ParamType { get; set; }

    /// <summary>
    /// Описание ключа (для чего он нужен).
    /// </summary>
    public string ParamDescription { get; set; }

    /// <summary>
    /// Тег ключа. Помогает быстро искать ключ по категории. Например, "Temp - временные ключи".
    /// </summary>
    public string ParamTag { get; set; }
}