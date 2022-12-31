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
    /// Должность.
    /// </summary>
    public string Job { get; set; }
}