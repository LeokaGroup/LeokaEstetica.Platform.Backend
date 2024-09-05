using AutoMapper;
using Dapper;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса шаблонов проекта.
/// </summary>
internal class ProjectManagementTemplateService : IProjectManagementTemplateService
{
    private readonly IProjectManagmentRepository _projectManagmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProjectManagementTemplateService> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectManagmentRepository">Репозиторий модуля УП.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="logger">Логгер.</param>
    public ProjectManagementTemplateService(IProjectManagmentRepository projectManagmentRepository,
     IMapper mapper,
     ILogger<ProjectManagementTemplateService> logger)
    {
        _projectManagmentRepository = projectManagmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectManagmentTaskTemplateResult>> GetProjectManagmentTemplatesAsync(
        long? templateId)
    {
        try
        {
            var items = (await _projectManagmentRepository.GetProjectManagmentTemplatesAsync(templateId))
                .AsList();
            
            // Разбиваем статусы на группы (кажда группа это отдельный шаблон со статусами).
            var templateIds = items.Select(x => x.TemplateId).Distinct().AsList();
            var result = new List<ProjectManagmentTaskTemplateResult>();

            foreach (var tid in templateIds)
            {
                // Выбираем все статусы определенного шаблона и добавляем в результат. 
                // Системные статусы тут не учитываем, с ними работаем позже при распределении уже.
                var templateStatuses = items.Where(x => x.TemplateId == tid);
                
                var templateTaskStatuses = _mapper.Map<IEnumerable<ProjectManagmentTaskStatusTemplateOutput>>(
                    templateStatuses);
                
                var resultItem = new ProjectManagmentTaskTemplateResult
                {
                    TemplateName = templateStatuses.FirstOrDefault()?.TemplateName,
                    ProjectManagmentTaskStatusTemplates = templateTaskStatuses
                };
                
                result.Add(resultItem);
            }

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SetProjectManagmentTemplateIdsAsync(List<ProjectManagmentTaskTemplateResult> templateStatuses)
    {
        try
        {
            if (templateStatuses is null || !templateStatuses.Any())
            {
                throw new InvalidOperationException(
                    "Невозможно проставить Id щаблонов статусам задач." +
                    $" TemplateStatuses: {JsonConvert.SerializeObject(templateStatuses)}");
            }

            // Находим в БД все статусы по их Id.
            var templateStatusIds = templateStatuses
                .SelectMany(x =>
                    (x.ProjectManagmentTaskStatusTemplates ?? new List<ProjectManagmentTaskStatusTemplateOutput>())
                    .Select(y => y.StatusId));

            var items = templateStatuses
                .SelectMany(x =>
                    (x.ProjectManagmentTaskStatusTemplates ?? new List<ProjectManagmentTaskStatusTemplateOutput>())
                    .Select(y => y));

            var statuses = (await _projectManagmentRepository.GetTemplateStatusIdsByStatusIdsAsync(templateStatusIds))
                .AsList();

            foreach (var ts in items)
            {
                // Пропускаем сейчас системные статусы, их получим позже.
                if (ts.IsSystemStatus && !ts.TemplateId.HasValue)
                {
                    continue;
                }
                
                var templateData = statuses.Find(x => x.StatusId == ts.StatusId);

                // Если не нашли такого статуса в таблице маппинга многие-ко-многим.
                if (templateData is null || templateData.StatusId <= 0 || templateData.TemplateId <= 0)
                {
                    throw new InvalidOperationException(
                        $"Не удалось получить шаблон, к которому принадлежит статус. Id статуса был: {ts.StatusId}");
                }

                ts.TemplateId = templateData.TemplateId;
            }
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }
}