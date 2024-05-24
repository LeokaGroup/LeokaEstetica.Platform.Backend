using LeokaEstetica.Platform.Models.Dto.Base.Chat;

namespace LeokaEstetica.Platform.Models.Dto.Chat.Output;

/// <summary>
/// Класс выходной модели диалога.
/// </summary>
public class DialogOutput : BaseDialogOutput
{
    /// <summary>
    /// Название диалога (фамилия и имя с кем ведется переписка).
    /// </summary>
    public string DialogName { get; set; }

    /// <summary>
    /// Имя собеседника.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Фамилия собеседника.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
}