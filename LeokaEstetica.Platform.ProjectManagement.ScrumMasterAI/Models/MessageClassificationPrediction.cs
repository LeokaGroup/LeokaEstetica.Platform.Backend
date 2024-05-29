using Microsoft.ML.Data;

namespace LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Models;

/// <summary>
/// Класс предсказателя нейросети при работе с сообщениями чата.
/// </summary>
public class MessageClassificationPrediction
{
    /// <summary>
    /// Предсказатель сообщения после обучения модели.
    /// Является результатом после классификации сообщения нейросетью.
    /// </summary>
    [ColumnName("PredictedLabel")]
    public string? Message;
}