namespace LeokaEstetica.Platform.Models.Dto.Chat.Output;

/// <summary>
/// Класс результата получения диалогов для ЛК.
/// </summary>
public class ProfileDialogResult
{
    /// <summary>
    /// Список диалогов проекта.
    /// </summary>
    public List<ProfileDialogOutput> Dialogs { get; set; }

    /// <summary>
    /// Тип события диалогов.
    /// </summary>
    public string ActionType { get; set; }
}