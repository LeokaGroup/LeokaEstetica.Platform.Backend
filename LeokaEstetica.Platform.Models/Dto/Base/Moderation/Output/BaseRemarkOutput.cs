namespace LeokaEstetica.Platform.Models.Dto.Base.Moderation.Output;

/// <summary>
/// Базовый класс выходной модели замечаний.
/// </summary>
public class BaseRemarkOutput
{
    /// <summary>
    /// PK.
    /// </summary>
    protected long RemarkId { get; set; }

    /// <summary>
    /// Название поля.
    /// </summary>
    protected string FieldName { get; set; }

    /// <summary>
    /// Текст замечания.
    /// </summary>
    protected string RemarkText { get; set; }

    /// <summary>
    /// Русское название замечания.
    /// </summary>
    protected string RussianName { get; set; }

    /// <summary>
    /// Id модератора.
    /// </summary>
    protected long ModerationUserId { get; set; }

    /// <summary>
    /// Дата создания замечания.
    /// </summary>
    protected DateTime DateCreated { get; set; }

    /// <summary>
    /// Id статуса замечания.
    /// </summary>
    protected int RemarkStatusId { get; set; }
}