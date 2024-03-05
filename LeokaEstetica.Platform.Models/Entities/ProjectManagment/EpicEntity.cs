namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей эпиков.
/// </summary>
public class EpicEntity
{
    /// <summary>
    /// PK.
    /// Id эпика.
    /// </summary>
    public long EpicId { get; set; }

    /// <summary>
    /// Название эпика.
    /// </summary>
    public string EpicName { get; set; }

    /// <summary>
    /// Описание эпика.
    /// </summary>
    public string EpicDescription { get; set; }

    /// <summary>
    /// Пользователь, который создал эпик.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Дата создания эпика.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата обновления эпика.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Пользователь, который обновил эпик.
    /// </summary>
    public long? UpdatedBy { get; set; }
}