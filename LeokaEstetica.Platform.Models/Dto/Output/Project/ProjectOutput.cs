using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели проекта.
/// </summary>
public class ProjectOutput : IFrontError
{
    /// <summary>
    /// Название проекта.
    /// </summary>
    public string ProjectName { get; set; }

    /// <summary>
    /// Описание проекта.
    /// </summary>
    public string ProjectDetails { get; set; }

    /// <summary>
    /// Изображение проекта.
    /// </summary>
    public string ProjectIcon { get; set; }
    
    /// <summary>
    /// PK.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Успешное ли сохранение.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Дата создания проекта.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }

    /// <summary>
    /// Название стадии проекта.
    /// </summary>
    public string StageName { get; set; }
    
    /// <summary>
    /// Системное название стадии проекта.
    /// </summary>
    public string StageSysName { get; set; }

    /// <summary>
    /// Id стадии проекта.
    /// </summary>
    public int StageId { get; set; }

    /// <summary>
    /// Признак видимости кнопки удаления.
    /// </summary>
    public bool IsVisibleDeleteButton { get; set; }

    /// <summary>
    /// Признак видимости кнопок управления проектом.
    /// </summary>
    public bool IsVisibleActionProjectButtons { get; set; }

    /// <summary>
    /// Признак видимости кнопки исключения участника из проекта.
    /// </summary>
    public bool IsVisibleActionDeleteProjectTeamMember { get; set; }

    /// <summary>
    /// Признак видимости кнопки покидания проекта.
    /// </summary>
    public bool IsVisibleActionLeaveProjectTeam { get; set; }
}