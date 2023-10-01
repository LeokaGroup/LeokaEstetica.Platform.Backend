namespace LeokaEstetica.Platform.Models.Dto.Chat.Output;

/// <summary>
/// Класс результата получения диалогов.
/// </summary>
public class ProjectDialogResult
{
    /// <summary>
    /// Список диалогов проекта.
    /// </summary>
    public List<DialogOutput> Dialogs { get; set; }

    /// <summary>
    /// Тип события диалогов.
    /// </summary>
    public string ActionType { get; set; }
}