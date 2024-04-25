using LeokaEstetica.Platform.Models.Entities.ProjectManagment;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели статусов истории.
/// </summary>
public class UserStoryStatusOutput : UserStoryStatusEntity
{
	public UserStoryStatusOutput(string statusName, string statusSysName) : base(statusName, statusSysName)
	{
	}
}