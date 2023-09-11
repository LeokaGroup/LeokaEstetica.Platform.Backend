namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Перечисление типов событий диалогов.
/// </summary>
public enum DialogActionType
{
    /// <summary>
    /// Нет события диалога.
    /// </summary>
    None,
    
    /// <summary>
    /// Событие получения всех диалогов.
    /// </summary>
    All,
    
    /// <summary>
    /// Событие получения конкретного диалога.
    /// </summary>
    Concrete,
    
    /// <summary>
    /// Событие получения сообщения диалога.
    /// </summary>
    Message
}