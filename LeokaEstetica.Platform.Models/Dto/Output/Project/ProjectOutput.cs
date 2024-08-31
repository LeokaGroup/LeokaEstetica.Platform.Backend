using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;

namespace LeokaEstetica.Platform.Models.Dto.Output.Project;

/// <summary>
/// Класс выходной модели проекта.
/// </summary>
public class ProjectOutput : ProjectRemarkResult, IFrontError
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
    /// Признак видимости кнопки покидания проекта.
    /// </summary>
    public bool IsVisibleActionLeaveProjectTeam { get; set; }
    
    /// <summary>
    /// Условия проекта.
    /// </summary>
    public string Conditions { get; set; }

    /// <summary>
    /// Требования проекта.
    /// </summary>
    public string Demands { get; set; }

    /// <summary>
    /// Признак видимости кнопки добавления проекта в архив.
    /// </summary>
    public bool IsVisibleActionAddProjectArchive { get; set; }

    /// <summary>
    /// Признак доступа.
    /// </summary>
    public bool IsAccess { get; set; }
    
    /// <summary>
    /// Название проекта для модуля УП (управление проектами).
    /// </summary>
    public string ProjectManagementName { get; set; }

    /// <summary>
    /// Префикс названия проекта для модуля УП (управление проектами).
    /// </summary>
    public string ProjectManagementNamePrefix { get; set; }
    
    /// <summary>
    /// Признак видимости проекта
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// Признак владельца проекта
    /// </summary>
    public bool IsVisibleProjectButton { get; set; }
}