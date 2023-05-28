namespace LeokaEstetica.Platform.Models.Dto.Base.Moderation.Output;

/// <summary>
/// Базовый класс выходной модели замечаний.
/// </summary>
public class BaseRemarkOutput
{
    /// <summary>
    /// PK.
    /// </summary>
    public long RemarkId { get; set; }

    /// <summary>
    /// Название поля.
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// Текст замечания.
    /// </summary>
    public string RemarkText { get; set; }

    /// <summary>
    /// Русское название замечания.
    /// </summary>
    public string RussianName { get; set; }

    /// <summary>
    /// Id модератора.
    /// </summary>
    public long ModerationUserId { get; set; }

    /// <summary>
    /// Дата создания замечания.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Id статуса замечания.
    /// </summary>
    public int RemarkStatusId { get; set; }
}