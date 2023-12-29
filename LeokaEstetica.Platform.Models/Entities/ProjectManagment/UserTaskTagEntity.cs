namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с таблицей тегов (меток) задачи пользователя.
/// </summary>
public class UserTaskTagEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int TagId { get; set; }

    /// <summary>
    /// Название тега.
    /// </summary>
    public string TagName { get; set; }

    /// <summary>
    /// Системное название тега.
    /// </summary>
    public string TagSysName { get; set; }
    
    /// <summary>
    /// Порядковый номер.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Описание метки (тега).
    /// </summary>
    public string TagDescription { get; set; }
}