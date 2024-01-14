namespace LeokaEstetica.Platform.Base.Enums;

/// <summary>
/// Абстракия для преставления типа SQL Enums в базе данных.
/// </summary>
public interface IEnum
{
    /// <summary>
    /// Значение.
    /// </summary>
    string Value { get; set; }

    /// <summary>
    /// Тип Enum в базе данных.
    /// </summary>
    string Type { get; }
}