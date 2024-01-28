using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Models.Extensions;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Base")]

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Класс для преставления типа SQL Enums в Postgres.
/// </summary>
internal class Enum : IEnum
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
        Value = value.ToString().ToSnakeCase();
    }

    /// <inheritdoc/>
    public string Value { get; set; }

    /// <inheritdoc/>
    public string Type { get; }
}