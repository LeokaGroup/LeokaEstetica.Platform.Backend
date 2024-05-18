using LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Enums;
using Microsoft.ML.Data;

namespace LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Models;

/// <summary>
/// Класс классификации нейросетью сообщений чата.
/// </summary>
public class MessageClassification
{
    /// <summary>
    /// Текст сообщения из чата, на которое нейросеть должна ответить.
    /// </summary>
    [LoadColumn(0)]
    public string? Message { get; set; }

    /// <summary>
    /// Id подключения клиента (уникальное подключение пользователя у сокета).
    /// </summary>
    // [LoadColumn(1)]
    // public Guid ConnectionId { get; set; }
    //
    // /// <summary>
    // /// Id пользователя, которое написал сообщение нейросети.
    // /// </summary>
    // [LoadColumn(2)]
    // public long CreatedBy { get; set; }
    //
    // /// <summary>
    // /// Дата создания сообщения (момент, когда нейросети написали сообщение).
    // /// </summary>
    // [LoadColumn(3)]
    // public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Тип события. Его использует нейросеть.
    /// </summary>
    // [LoadColumn(4)]
    // public ScrumMasterAiEventTypeEnum ScrumMasterAiEventType { get; set; }
}