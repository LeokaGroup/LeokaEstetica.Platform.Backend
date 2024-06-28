using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectTeam;

/// <summary>
/// Класс выходной модели участника команды проекта.
/// </summary>
public class ProjectTeamMemberOutput : IFrontSuccess, IFrontError
{
    /// <summary>
    /// PK.
    /// </summary>
    public long MemberId { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long TeamId { get; set; }
    
    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }
    
    /// <summary>
    /// Дата присоединения участника к команде.
    /// </summary>
    public DateTime Joined { get; set; }

    /// <summary>
    /// Id вакансии.
    /// </summary>
    public long VacancyId { get; set; }

    /// <summary>
    /// Текст успеха, который будем выводить на фронт.
    /// </summary>
    public string SuccessMessage { get; set; }
    
    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }
}