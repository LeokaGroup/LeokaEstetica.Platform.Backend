namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сопоставляется с табюлицей статусов историй.
/// </summary>
public class UserStoryStatusEntity
{
    public UserStoryStatusEntity(string statusName, string statusSysName)
    {
        StatusName = statusName;
        StatusSysName = statusSysName;
    }
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