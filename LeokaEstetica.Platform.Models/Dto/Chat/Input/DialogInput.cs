namespace LeokaEstetica.Platform.Models.Dto.Chat.Input;

/// <summary>
/// Класс входной модели диалога.
/// </summary>
public class DialogInput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="discussionType">Тип предмета обсуждения. Проект, вакансия и тд.</param>
    public DialogInput(string discussionType)
    {
        DiscussionType = discussionType;
    }

    /// <summary>
    /// Id диалога.
    /// </summary>
    public long? DialogId { get; set; }

    /// <summary>
    /// Тип предмета обсуждения. Проект, вакансия и тд.
    /// </summary>
    public string DiscussionType { get; set; }

    /// <summary>
    /// Id предмета обсуждения (Id проекта или вакансии).
    /// </summary>
    public long? DiscussionTypeId { get; set; }

    /// <summary>
    /// Признак ручного создания диалога (по кнопке и тд).
    /// </summary>
    public bool isManualNewDialog { get; set; }
}