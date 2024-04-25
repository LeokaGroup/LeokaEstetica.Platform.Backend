namespace LeokaEstetica.Platform.Models.Entities.ProjectManagment;

/// <summary>
/// Класс сущности расширяющей сущность задачи.
/// </summary>
public class ProjectTaskExtendedEntity : ProjectTaskEntity
{
	public ProjectTaskExtendedEntity(string name) : base(name)
	{
	}

	/// <summary>
	/// Префикс номера задачи.
	/// </summary>
	public string TaskIdPrefix { get; set; }
}