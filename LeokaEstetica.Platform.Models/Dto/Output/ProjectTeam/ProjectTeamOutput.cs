namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectTeam;

/// <summary>
/// Класс выходной модели команды проекта.
/// </summary>
public class ProjectTeamOutput
{
    /// <summary>
    /// Id участника проекта.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Название вакансии.
    /// </summary>
    public string VacancyName { get; set; }

    /// <summary>
    /// Участник (ФИО если заполнено или логин). 
    /// </summary>
    public string Member { get; set; }

    /// <summary>
    /// В проекте с.
    /// </summary>
    public string Joined { get; set; }
    
    /// <summary>
    /// Признак видимости кнопки исключения участника из проекта.
    /// </summary>
    public bool IsVisibleActionDeleteProjectTeamMember { get; set; }
    
    /// <summary>
    /// Роль участника команды проекта. Заполняется в произвольном виде владельцем проекта.
    /// </summary>
    public string? Role { get; set; }
}