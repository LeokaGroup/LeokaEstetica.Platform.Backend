using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция сервиса, который распределяет задачи по статусам.
/// </summary>
public interface IDistributionStatusTaskService
{
    /// <summary>
    /// Метод распределяет задачи по статусам шаблона проекта.
    /// Системные статусы также учитываются.
    /// </summary>
    /// <param name="projectManagmentTaskStatusTemplates">Статуса шаблона.</param>
    /// <param name="tasks">Список задач до модификации данных.</param>
    /// <param name="modifyTaskStatuseType">Тип модификации результата.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="paginatorStatusId">Id статуса пагинации.</param>
    /// <param name="strategy">Выбранная стратегия представления.</param>
    /// <param name="page">Страница.</param>
    Task<List<ProjectManagmentTaskStatusTemplateOutput>> DistributionStatusTaskAsync(
        List<ProjectManagmentTaskStatusTemplateOutput> projectManagmentTaskStatusTemplates,
        List<ProjectTaskExtendedEntity> tasks,
        ModifyTaskStatuseTypeEnum modifyTaskStatuseType, long projectId, int? paginatorStatusId, string strategy,
        int page = 1);
}