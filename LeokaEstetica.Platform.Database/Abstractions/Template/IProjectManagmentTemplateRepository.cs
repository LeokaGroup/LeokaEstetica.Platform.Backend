using LeokaEstetica.Platform.Models.Entities.Template;

namespace LeokaEstetica.Platform.Database.Abstractions.Template;

/// <summary>
/// Абстракция шаблонов модуля УП.
/// </summary>
public interface IProjectManagmentTemplateRepository
{
    /// <summary>
    /// TODO: Изменить на получение шаблона из репозитория конфигов настроек проектов.
    /// Метод получает шаблон проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Id шаблона.</returns>
    Task<int?> GetProjectTemplateIdAsync(long projectId);
    
    /// <summary>
    /// Метод получает список Id статусов, которые принадлежат шаблону.
    /// </summary>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Список Id статусов.</returns>
    Task<IEnumerable<long>> GetTemplateStatusIdsAsync(int templateId);

    /// <summary>
    /// Метод получает статусы шаблона проекта.
    /// </summary>
    /// <param name="statusIds">Список Id статусов шаблона.</param>
    /// <returns>Список статусов шаблона.</returns>
    Task<IEnumerable<ProjectManagmentTaskStatusTemplateEntity>>
        GetTaskTemplateStatusesAsync(IEnumerable<long> statusIds);

    /// <summary>
    /// Метод получает статус шаблона по системному имени.
    /// </summary>
    /// <param name="associationStatusSysName">Название статуса, с которым ассоциируется системное название.</param>
    /// <returns>Данные статуса.</returns>
    Task<ProjectManagmentTaskStatusTemplateEntity> GetProjectManagementStatusBySysNameAsync(
        string associationStatusSysName);

    /// <summary>
    /// Метод создает статус шаблона.
    /// </summary>
    /// <param name="statusTemplateEntity">Данные нового статуса.</param>
    /// <returns>Id добавленного кастомного статуса.</returns>
    Task<long> CreateProjectManagmentTaskStatusTemplateAsync(
        ProjectManagementUserStatuseTemplateEntity statusTemplateEntity);
    
    /// <summary>
    /// Метод получает максимальный Position у кастомных статусов шаблонов пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Позиция последнего статуса.</returns>
    Task<int> GetLastPositionUserStatusTemplateAsync(long userId);

    /// <summary>
    /// Метод добавляет кастомный статус в маппинг статусов шаблонов.
    /// </summary>
    /// <param name="statusId">Id статуса.</param>
    /// <param name="templateId">Id шаблона.</param>
    Task CreateProjectManagmentTaskStatusIntermediateTemplateAsync(long statusId, int templateId);

    /// <summary>
    /// Метод получает название статуса по TaskStatusId.
    /// </summary>
    /// <param name="taskStatusId">Id статуса задачи.</param>
    /// <returns>Название статуса.</returns>
    Task<string> GetStatusNameByTaskStatusIdAsync(int taskStatusId);
}