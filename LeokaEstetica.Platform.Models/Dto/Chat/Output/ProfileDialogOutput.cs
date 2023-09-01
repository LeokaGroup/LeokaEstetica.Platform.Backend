namespace LeokaEstetica.Platform.Models.Dto.Chat.Output;

/// <summary>
/// Класс выходной модели диалога профиля пользователя.
/// </summary>
public class ProfileDialogOutput : BaseDialogOutput
{
    /// <summary>
    /// Название проекта.
    /// </summary>
    public string ProjectName { get; set; }
}