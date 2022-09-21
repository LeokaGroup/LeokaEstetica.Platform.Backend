namespace LeokaEstetica.Platform.Models.Dto.Input.Header;

/// <summary>
/// Класс входной модели хидера.
/// </summary>
public class HeaderInput
{
    /// <summary>
    /// Тип хидера. Для лендоса или каталогов.
    /// </summary>
    public HeaderTypeEnum HeaderType { get; set; }
}

/// <summary>
/// Перечисление типов хидеров.
/// </summary>
public enum HeaderTypeEnum
{
    Landing = 1,
    Catalog = 2
}