using LeokaEstetica.Platform.Core.Enums;

namespace LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.IntegrationEvents;

/// <summary>
/// Класс события классификации сообщений нейросетью.
/// </summary>
public class MessageClassificationEvent
{
    /// <summary>
    /// Текст сообщения из чата, на которое нейросеть должна ответить.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// TODO: Пока что всегда -1, так как это Id нейросети.
    /// TODO: Если нейросетей станет несколько, то будем получать из БД.
    /// Id пользователя, которое написал сообщение нейросети.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Тип события. Его использует нейросеть.
    /// </summary>
    public ScrumMasterAiEventTypeEnum ScrumMasterAiEventType { get; set; }

    /// <summary>
    /// TODO: Если Id подключения успеет измениться (пользователь уйдет со страницы), то пользователь не получит ответ.
    /// TODO: Кривое место, важно будет рефачить это, чтоб всегда получал ответ
    /// TODO: (Гуид не подойдет уже, так как гуид создается каждый раз при входе на страницы разные на фронте).
    /// <param name="connectionId">Id подключения сокетов.</param>
    /// </summary>
    public string? ConnectionId { get; set; }

    /// <summary>
    /// Id диалога.
    /// </summary>
    public long DialogId { get; set; }
}