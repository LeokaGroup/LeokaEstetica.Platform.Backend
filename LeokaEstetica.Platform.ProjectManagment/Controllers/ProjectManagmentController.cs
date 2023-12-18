using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.ProjectManagment.ValidationModels;
using LeokaEstetica.Platform.ProjectManagment.Validators;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.ProjectManagment.Controllers;

/// <summary>
/// Контроллер управления проектами.
/// </summary>
[ApiController]
[Route("project-managment")]
[AuthFilter]
public class ProjectManagmentController : BaseController
{
    private readonly IProjectService _projectService;
    private readonly IProjectManagmentService _projectManagmentService;
    private readonly IMapper _mapper;
    private readonly ILogger<ProjectManagmentController> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectService">Сервис проектов пользователей.</param>
    /// <param name="projectManagmentService">Сервис управления проектами.</param>
    /// <param name="mapper">Маппер.</param>
    /// <param name="logger">Логгер.</param>
    public ProjectManagmentController(IProjectService projectService,
        IProjectManagmentService projectManagmentService,
        IMapper mapper,
        ILogger<ProjectManagmentController> logger)
    {
        _projectService = projectService;
        _projectManagmentService = projectManagmentService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// TODO: Подумать, стоит ли выводить в рабочее пространство архивные проекты и те, что находятся на модерации.
    /// Метод получает список проектов пользователя.
    /// </summary>
    /// <returns>Список проектов пользователя.</returns>
    [HttpGet]
    [Route("user-projects")]
    [ProducesResponseType(200, Type = typeof(UserProjectResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UserProjectResultOutput> UserProjectsAsync()
    {
        var result = await _projectService.UserProjectsAsync(GetUserName(), false);

        return result;
    }

    /// <summary>
    /// Метод получает список стратегий представления рабочего пространства.
    /// </summary>
    /// <returns>Список стратегий.</returns>
    [HttpGet]
    [Route("view-strategies")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ViewStrategyOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ViewStrategyOutput>> GetViewStrategiesAsync()
    {
        var items = await _projectManagmentService.GetViewStrategiesAsync();
        var result = _mapper.Map<IEnumerable<ViewStrategyOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод получает элементы верхнего меню (хидера).
    /// </summary>
    /// <returns>Список элементов.</returns>
    [HttpGet]
    [Route("header")]
    [ProducesResponseType(200, Type = typeof(List<ProjectManagmentHeaderResult>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<List<ProjectManagmentHeaderResult>> GetHeaderItemsAsync()
    {
        // Получаем необработанные списки. Доп.списки пока не заполнены.
        var unprocessedItems = await _projectManagmentService.GetHeaderItemsAsync();
        var mapItems = _mapper.Map<IEnumerable<ProjectManagmentHeaderOutput>>(unprocessedItems);
        
        // Наполняем доп.списки.
        var result = await _projectManagmentService.ModifyHeaderItemsAsync(mapItems);

        return result;
    }

    /// <summary>
    /// Метод получает список шаблонов задач, которые пользователь может выбрать перед переходом в рабочее пространство.
    /// </summary>
    /// <returns>Список шаблонов задач.</returns>
    [HttpGet]
    [Route("templates")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectManagmentTaskTemplateResult>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectManagmentTaskTemplateResult>> GetProjectManagmentTemplatesAsync()
    {
        var items = await _projectManagmentService.GetProjectManagmentTemplatesAsync(null);
        var result = _mapper.Map<IEnumerable<ProjectManagmentTaskTemplateResult>>(items);
        var resultItems = result.ToList();
        
        // Проставляем Id шаблона статусам.
        await _projectManagmentService.SetProjectManagmentTemplateIdsAsync(resultItems);

        return resultItems;
    }

    /// <summary>
    /// Метод получает конфигурацию рабочего пространства по выбранному шаблону.
    /// Под конфигурацией понимаются основные элементы рабочего пространства (набор задач, статусов, фильтров, колонок и тд)
    /// если выбранный шаблон это предполагает.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="strategy">Выбранная стратегия представления.</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Данные конфигурации рабочего пространства.</returns>
    [HttpGet]
    [Route("config-workspace-template")]
    [ProducesResponseType(200, Type = typeof(ProjectManagmentWorkspaceResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectManagmentWorkspaceResult> GetConfigurationWorkSpaceBySelectedTemplateAsync(
        [FromQuery] long projectId, [FromQuery] string strategy, [FromQuery] int templateId)
    {
        var validator = await new GetConfigurationValidator().ValidateAsync(
            new GetConfigurationValidationModel(projectId, strategy, templateId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка получения конфигурации рабочего пространства.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetConfigurationWorkSpaceBySelectedTemplateAsync(
            projectId, strategy, templateId, GetUserName());

        return result;
    }

    /// <summary>
    /// Метод получает детали задачи.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные задачи.</returns>
    [HttpGet]
    [Route("task")]
    [ProducesResponseType(200, Type = typeof(ProjectManagmentTaskOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectManagmentTaskOutput> GetTaskDetailsByTaskIdAsync([FromQuery] long projectTaskId,
        [FromQuery] long projectId)
    {
        var result = await _projectManagmentService.GetTaskDetailsByTaskIdAsync(projectTaskId, GetUserName(),
            projectId);

        return result;
    }

    /// <summary>
    /// Метод создает задачу проекта.
    /// </summary>
    /// <param name="projectManagementTaskInput">Входная модель.</param>
    [HttpPost]
    [Route("task")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task CreateProjectTaskAsync([FromBody] CreateProjectManagementTaskInput projectManagementTaskInput)
    {
        var validator = await new CreateProjectManagementTaskValidator().ValidateAsync(projectManagementTaskInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException($"Ошибка создания задачи проекта {projectManagementTaskInput.ProjectId}.",
                exceptions);
            _logger.LogError(ex, ex.Message);
            
            throw ex;
        }
    }

    /// <summary>
    /// Метод получает список приоритетов задачи.
    /// </summary>
    /// <returns>Список приоритетов задачи.</returns>
    [HttpGet]
    [Route("priorities")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TaskPriorityOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<TaskPriorityOutput>> GetTaskPrioritiesAsync()
    {
        var items = await _projectManagmentService.GetTaskPrioritiesAsync();
        var result = _mapper.Map<IEnumerable<TaskPriorityOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод получает список типов задач.
    /// </summary>
    /// <returns>Список типов задач.</returns>
    [HttpGet]
    [Route("task-types")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TaskTypeOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<TaskTypeOutput>> GetTaskTypesAsync()
    {
        var items = await _projectManagmentService.GetTaskTypesAsync();
        var result = _mapper.Map<IEnumerable<TaskTypeOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод получает список тегов для выбора в задаче.
    /// </summary>
    /// <returns>Список тегов.</returns>
    [HttpGet]
    [Route("task-tags")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TaskTagOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<TaskTagOutput>> GetTaskTagsAsync()
    {
        var items = await _projectManagmentService.GetTaskTagsAsync();
        var result = _mapper.Map<IEnumerable<TaskTagOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод получает список статусов задачи для выбора.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список статусов.</returns>
    [HttpGet]
    [Route("task-statuses")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TaskStatusOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<TaskStatusOutput>> GetTaskStatusesAsync([FromQuery] long projectId)
    {
        var validator = await new GetTaskStatusValidator().ValidateAsync(
            new GetTaskStatusValidationModel(projectId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка получения статусов задачи проекта.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            throw ex;
        }

        var items = await _projectManagmentService.GetTaskStatusesAsync(projectId, GetUserName());
        var result = _mapper.Map<IEnumerable<TaskStatusOutput>>(items);

        return result;
    }
}