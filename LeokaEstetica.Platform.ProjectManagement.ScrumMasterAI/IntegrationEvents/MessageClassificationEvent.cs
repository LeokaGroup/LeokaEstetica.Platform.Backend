using LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Enums;

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
    /// Id пользователя, которое написал сообщение нейросети.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Дата создания сообщения (момент, когда нейросети написали сообщение).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Тип события. Его использует нейросеть.
    /// </summary>
    public ScrumMasterAiEventTypeEnum ScrumMasterAiEventType { get; set; }

    /// <summary>
    /// TODO: Если токен успеет измениться (пользователь уйдет со страницы), то пользователь не получит ответ.
    /// TODO: Кривое место, важно будет рефачить это, чтоб всегда получал ответ
    /// TODO: (Гуид не подойдет уже, так как гуид создается каждый раз при входе на страницы разные на фронте).
    /// Токен пользователя. Нужно для отправки ему ответа нейросетью.
    /// </summary>
    public string? Token { get; set; }
}