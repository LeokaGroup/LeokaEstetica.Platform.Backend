namespace LeokaEstetica.Platform.Models.Dto.Chat.Output;

/// <summary>
/// Класс выходной модели диалога профиля пользователя.
/// </summary>
public class ProfileDialogOutput : DialogOutput
{
    /// <summary>
    /// Название проекта.
    /// </summary>
    public string ProjectName { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}