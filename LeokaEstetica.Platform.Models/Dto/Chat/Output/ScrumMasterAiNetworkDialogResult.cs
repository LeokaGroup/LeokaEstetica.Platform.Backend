namespace LeokaEstetica.Platform.Models.Dto.Chat.Output;

/// <summary>
/// Класс результата получения диалогов.
/// </summary>
public class ScrumMasterAiNetworkDialogResult
{
    /// <summary>
    /// Список диалогов.
    /// </summary>
    public List<ScrumMasterAiNetworkDialogOutput> Dialogs { get; set; }

    /// <summary>
    /// Тип события диалогов.
    /// </summary>
    public string ActionType { get; set; }
}