namespace LeokaEstetica.Platform.Core.Enums;

/// <summary>
/// Перечисление типа ивента нейросети.
/// </summary>
public enum ScrumMasterAiEventTypeEnum
{
    /// <summary>
    /// Неизвестное событие.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Событие сообщения, которое написали из чата для нейросети.
    /// </summary>
    Message = 1,
    
    /// <summary>
    /// Событие анализа (его создает отдельная джоба, которая отправляет данные для нейросети).
    /// </summary>
    Analysis = 2
}