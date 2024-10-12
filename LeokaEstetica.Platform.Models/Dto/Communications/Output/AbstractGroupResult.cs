namespace LeokaEstetica.Platform.Models.Dto.Communications.Output;

/// <summary>
/// Класс результата объектов группы абстрактной области.
/// </summary>
public class AbstractGroupResult
{
    /// <summary>
    /// Название группы объектов.
    /// </summary>
    public string? GroupName { get; set; }

    /// <summary>
    /// Объекты группы.
    /// </summary>
    public IEnumerable<AbstractGroupOutput>? Objects { get; set; }
}