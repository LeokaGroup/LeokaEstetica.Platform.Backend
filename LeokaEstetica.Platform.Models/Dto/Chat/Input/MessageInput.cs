namespace LeokaEstetica.Platform.Models.Dto.Chat.Input;

/// <summary>
/// Класс входной модели сообщений.
/// </summary>
public class MessageInput
{
    /// <summary>
    /// Сообщение.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Id диалога.
    /// </summary>
    public long DialogId { get; set; }
}