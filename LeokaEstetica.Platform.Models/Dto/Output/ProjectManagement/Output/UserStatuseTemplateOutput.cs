namespace LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;

/// <summary>
/// Класс выходной модели статусов задач пользователя.
/// </summary>
public class UserStatuseTemplateOutput
{
    /// <summary>
    /// Id статуса.
    /// </summary>
    public long StatusId { get; set; }
    
    /// <summary>
    /// Название статуса.
    /// </summary>
    public string StatusName { get; set; }

    /// <summary>
    /// Системное название статуса.
    /// </summary>
    public string StatusSysName { get; set; }

    /// <summary>
    /// Позиция.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Описание статуса шаблона.
    /// </summary>
    public string StatusDescription { get; set; }
}