namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// TODO: Выпилить и заменить на DTO.
/// Класс сопоставляется с табюлицей статусов историй.
/// </summary>
public class UserStoryStatusEntity
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
}