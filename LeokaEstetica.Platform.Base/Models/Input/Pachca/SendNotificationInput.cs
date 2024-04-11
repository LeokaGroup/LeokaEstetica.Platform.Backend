namespace LeokaEstetica.Platform.Base.Models.Input.Pachca;

/// <summary>
/// Класс входной модели исключений для пачки.
/// </summary>
public class SendNotificationInput
{
    /// <summary>
    /// Данные исключения.
    /// </summary>
    public IEnumerable<EmbedsItem> Embeds { get; set; }
}

public class EmbedsItem
{
    /// <summary>
    /// Заголовок.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Текст исключения.
    /// </summary>
    public string Description { get; set; }
}