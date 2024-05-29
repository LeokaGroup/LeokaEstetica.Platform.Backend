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
    /// Ответ нейросети.
    /// </summary>
    [LoadColumn(1)]
    public string? Response { get; set; }
}