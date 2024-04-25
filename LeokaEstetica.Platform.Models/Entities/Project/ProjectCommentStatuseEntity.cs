namespace LeokaEstetica.Platform.Models.Entities.Project;

/// <summary>
/// Класс сопоставляется с таблицей статусов комментариев проектов.
/// </summary>
public class ProjectCommentStatuseEntity
{
    public ProjectCommentStatuseEntity(string statusName, string statusSysName)
    {
		StatusName = statusName;
		StatusSysName = statusSysName;
	}
    /// <summary>
    /// PK.
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Название статуса.
    /// </summary>
    public string StatusName { get; set; }

    /// <summary>
    /// Системное название статуса.
    /// </summary>
    public string StatusSysName { get; set; }
}