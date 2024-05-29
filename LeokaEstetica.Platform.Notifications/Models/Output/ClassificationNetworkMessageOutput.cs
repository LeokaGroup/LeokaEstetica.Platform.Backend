namespace LeokaEstetica.Platform.Notifications.Models.Output;

/// <summary>
/// Класс выходной модели ответа нейросети Scrum Master AI.
/// </summary>
public class ClassificationNetworkMessageOutput
{
    /// <summary>
    /// Ответ нейросети.
    /// </summary>
    public string? Response { get; set; }

    /// <summary>
    /// Тип ивента.
    /// </summary>
    public string? ScrumMasterAiEventType { get; set; }

    /// <summary>
    /// Признак сообщения текущего пользователя.
    /// Если специалисту тех.поддержки придется подключиться к диалогу, то он увидит ответы нейросети.
    /// </summary>
    public bool IsMyMessage { get; set; }

    /// <summary>
    /// Id подключения сокетов.
    /// </summary>
    public string? ConnectionId { get; set; }

    /// <summary>
    /// Id диалога.
    /// </summary>
    public long DialogId { get; set; }
}