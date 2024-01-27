namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Класс для преставления типа SQL Enums в Postgres.
/// </summary>
public class Enum : IEnum
{
    /// <summary>
    /// Тип Enum для типов БВС.
    /// </summary>
    public const string ObjectTagType = "object_tag_type_enum";

    /// <summary>
    /// Конструктор по дефолту.
    /// </summary>
    /// <param name="type">Тип Enum в базе данных.</param>
    public Enum(string type = null)
    {
        Type = type;
    }

    /// <summary>
    /// Конструктор .
    /// </summary>
    /// <param name="value">Значение.</param>
    public Enum(ObjectTagTypeEnum value)
    {
        Type = ObjectTagType;
        Value = ToSnakeCase(value.ToString());
    }

    /// <inheritdoc/>
    public string Value { get; set; }

    /// <inheritdoc/>
    public string Type { get; }

    /// <summary>
    /// Этот метод дубликат из Base, так как ссылки не добавить.
    /// Конвертирует строковое значение в snake-case.
    /// </summary>
    /// <param name="value"> Значение для конвертации. </param>
    /// <returns> Конвертированное значение. </returns>
    public static string ToSnakeCase(string value)
    {
        return string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
    }
}