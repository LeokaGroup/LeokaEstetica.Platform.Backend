using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Database.Abstractions.Template;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.Document;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.ProjectManagement.Validators;
using LeokaEstetica.Platform.ProjectManagment.ValidationModels;
using LeokaEstetica.Platform.ProjectManagment.Validators;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Services.Abstractions.User;
using LeokaEstetica.Platform.Services.Factors;
using LeokaEstetica.Platform.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.ProjectManagement.Controllers;

/// <summary>
/// TODO: Разнести на разные контроллеры ендпоинты, которые точно нужно разделить по предметной области.
/// TODO: Фронт аналогично потребуется разделять.
/// Контроллер управления проектами.
/// </summary>
[ApiController]
[Route("project-management")]
[AuthFilter]
public class ProjectManagmentController : BaseController
{
    private readonly IProjectService _projectService;
    private readonly IProjectManagmentService _projectManagmentService;
    private readonly IMapper _mapper;
    private readonly ILogger<ProjectManagmentController> _logger;
    private readonly IUserService _userService;
    private readonly Lazy<IDiscordService> _discordService;
    private readonly Lazy<IProjectManagmentTemplateRepository> _projectManagmentTemplateRepository;
    private readonly IProjectManagementTemplateService _projectManagementTemplateService;
    private readonly IProjectManagmentRepository _projectManagmentRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectService">Сервис проектов пользователей.</param>
    /// <param name="projectManagmentService">Сервис управления проектами.</param>
    /// <param name="mapper">Маппер.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="userService">Сервис пользователей.</param>
    /// <param name="discordService">Сервис дискорда.</param>
    /// <param name="projectManagmentTemplateRepository">Репозиторий шаблонов проектов.</param>
    /// <param name="projectManagementTemplateService">Сервис шаблонов проекта.</param>
    /// <param name="projectManagmentRepository">Репозиторий модуля УП</param>
    public ProjectManagmentController(IProjectService projectService,
        IProjectManagmentService projectManagmentService,
        IMapper mapper,
        ILogger<ProjectManagmentController> logger,
        IUserService userService,
        Lazy<IDiscordService> discordService,
        Lazy<IProjectManagmentTemplateRepository> projectManagmentTemplateRepository,
        IProjectManagementTemplateService projectManagementTemplateService,
        IProjectManagmentRepository projectManagmentRepository)
    {
        _projectService = projectService;
        _projectManagmentService = projectManagmentService;
        _mapper = mapper;
        _logger = logger;
        _userService = userService;
        _discordService = discordService;
        _projectManagmentTemplateRepository = projectManagmentTemplateRepository;
        _projectManagementTemplateService = projectManagementTemplateService;
        _projectManagmentRepository = projectManagmentRepository;
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
    /// Метод получает элементы панели.
    /// Панель это и хидер модуля УП и левое выдвижное меню и меню документации проектов.
    /// </summary>
    /// <returns>Список элементов панели.</returns>
    [HttpGet]
    [Route("panel")]
    [ProducesResponseType(200, Type = typeof(GetPanelResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<GetPanelResult> GetPanelItemsAsync()
    {
        // Получаем необработанные списки. Доп.списки пока не заполнены.
        var unprocessedItems = await _projectManagmentService.GetPanelItemsAsync();
        var mapItems = _mapper.Map<IEnumerable<PanelOutput>>(unprocessedItems);
        
        // Наполняем доп.списки.
        var result = await _projectManagmentService.ModifyPanelItemsAsync(mapItems);

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
        var items = await _projectManagementTemplateService.GetProjectManagmentTemplatesAsync(null);
        var result = _mapper.Map<IEnumerable<ProjectManagmentTaskTemplateResult>>(items);
        var resultItems = result.ToList();
        
        // Проставляем Id шаблона статусам.
        await _projectManagementTemplateService.SetProjectManagmentTemplateIdsAsync(resultItems);

        return resultItems;
    }

    /// <summary>
    /// Метод получает конфигурацию рабочего пространства по выбранному шаблону.
    /// Под конфигурацией понимаются основные элементы рабочего пространства (набор задач, статусов, фильтров, колонок и тд)
    /// если выбранный шаблон это предполагает.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="paginatorStatusId">Id статуса, для которого нужно применить пагинатор.
    /// Если он null, то пагинатор применится для задач всех статусов шаблона.</param>
    /// <param name="modifyTaskStatuseType">Компонент, данные которого будем модифицировать.</param>
    /// <param name="page">Номер страницы.</param>
    /// <returns>Данные конфигурации рабочего пространства.</returns>
    [HttpGet]
    [Route("config-workspace-template")]
    [ProducesResponseType(200, Type = typeof(ProjectManagmentWorkspaceResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectManagmentWorkspaceResult> GetConfigurationWorkSpaceBySelectedTemplateAsync(
        [FromQuery] long projectId, [FromQuery] int? paginatorStatusId,
        [FromQuery] ModifyTaskStatuseTypeEnum modifyTaskStatuseType, [FromQuery] int page = 1)
    {
        var validator = await new GetConfigurationValidator().ValidateAsync(
            new GetConfigurationValidationModel(projectId));

        if (validator.Errors.Any()) 
        {
            var exceptions = new List<InvalidOperationException>(); 

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка получения конфигурации рабочего пространства.", exceptions);
            _logger.LogError(ex, ex.Message);
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetConfigurationWorkSpaceBySelectedTemplateAsync(
            projectId, GetUserName(), paginatorStatusId, modifyTaskStatuseType, page);

        return result;
    }

    /// <summary>
    /// Метод получает детали задачи.
    /// Детали задачи могут быть разные, в зависимости от типа задачи, который передали.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskDetailType">Тип детализации.</param>
    /// <returns>Данные задачи.</returns>
    [HttpGet]
    [Route("task-details")]
    [ProducesResponseType(200, Type = typeof(ProjectManagmentTaskOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectManagmentTaskOutput> GetTaskDetailsByTaskIdAsync([FromQuery] string projectTaskId,
        [FromQuery] long projectId, [FromQuery] TaskDetailTypeEnum taskDetailType)
    {
        var validator = await new TaskDetailValidator().ValidateAsync((projectTaskId, projectId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка получения деталей задачи. " +
                                            $"ProjectTaskId: {projectTaskId}. " +
                                            $"ProjectId: {projectId}. " +
                                            $"TaskDetailType: {taskDetailType}.", exceptions);
            _logger.LogError(ex, ex.Message);
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }
        
        var result = await _projectManagmentService.GetTaskDetailsByTaskIdAsync(projectTaskId, GetUserName(),
            projectId, taskDetailType);

        return result;
    }

    /// <summary>
    /// Метод создает задачу проекта.
    /// </summary>
    /// <param name="projectManagementTaskInput">Входная модель.</param>
    /// <returns>Выходная модель.</returns>
    [HttpPost]
    [Route("task")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CreateProjectManagementTaskOutput> CreateProjectTaskAsync(
        [FromBody] CreateProjectManagementTaskInput projectManagementTaskInput)
    {
        var validator = await new CreateProjectManagementTaskValidator().ValidateAsync(projectManagementTaskInput);
        
        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException($"Ошибка создания задачи проекта {projectManagementTaskInput.ProjectId}. " +
                                            $"Тип задачи был: {projectManagementTaskInput.TaskTypeId}. " +
                                            $"Название задачи: {projectManagementTaskInput.Name}. " +
                                            $"Статус задачи: {projectManagementTaskInput.TaskStatusId}.",
                exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);

            return new CreateProjectManagementTaskOutput { Errors = validator.Errors };
        }

        var result = await _projectManagmentService.CreateProjectTaskAsync(projectManagementTaskInput, GetUserName());

        return result;
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
    /// Метод получает список тегов проекта для выбора в задаче.
    /// </summary>
    /// <returns>Список тегов.</returns>
    [HttpGet]
    [Route("project-tags")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectTagOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectTagOutput>> GetProjectTagsAsync([FromQuery] long projectId)
    {
        var items = await _projectManagmentService.GetProjectTagsAsync(projectId);
        var result = _mapper.Map<IEnumerable<ProjectTagOutput>>(items);

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
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetTaskStatusesAsync(projectId, GetUserName());

        return result;
    }

    /// <summary>
    /// Метод получает пользователей, которые могут быть выбраны в качестве исполнителя или наблюдателей задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список пользователей.</returns>
    [HttpGet]
    [Route("select-task-people")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TaskPeopleOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<TaskPeopleOutput>> GetSelectTaskExecutorsAsync([FromQuery] long projectId)
    {
        var validator = await new GetTaskExecutorValidator().ValidateAsync(
            new GetTaskExecutorValidationModel(projectId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка получения исполнителей или наблюдателей задачи проекта.",
                exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var items = await _projectManagmentService.GetSelectTaskExecutorsAsync(projectId, GetUserName());
        var result = _mapper.Map<IEnumerable<TaskPeopleOutput>>(items);
        result = await _userService.SetUserCodesAsync(result.ToList());

        return result;
    }

    /// <summary>
    /// Метод создает метку (тег) проекта.
    /// </summary>
    /// <param name="projectTagInput">Входная модель.</param>
    [HttpPost]
    [Route("project-tag")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task CreateProjectTagAsync([FromBody] ProjectTagInput projectTagInput)
    {
        var validator = await new CreateUserTaskTagValidator().ValidateAsync(projectTagInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка создания метки (тега).",
                exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.CreateProjectTagAsync(projectTagInput.TagName,
            projectTagInput.TagDescription, projectTagInput.ProjectId, GetUserName());
    }

    /// <summary>
    /// Метод получает список статусов для выбора для создания нового статуса.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список статусов.</returns>
    [HttpGet]
    [Route("select-create-task-statuses")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TaskStatusOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<TaskStatusOutput>> GetSelectableTaskStatusesAsync([FromQuery] long projectId)
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
            
            var ex = new AggregateException("Ошибка получения статусов для создания нового статуса.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }
        
        // Получаем шаблон, по которому управляется проект.
        var templateId = await _projectManagmentTemplateRepository.Value.GetProjectTemplateIdAsync(projectId);

        if (!templateId.HasValue || templateId.Value <= 0)
        {
            throw new InvalidOperationException($"Не удалось получить шаблон проекта. ProjectId: {projectId}");
        }

        var items = (await _projectManagmentService.GetSelectableTaskStatusesAsync(projectId, templateId.Value))
            .ToList();
        var result = _mapper.Map<IEnumerable<TaskStatusOutput>>(items);
        var resultItems = result.ToList();
        
        // Проставляем шаблон для выходной модели.
        foreach (var s in resultItems)
        {
            s.TemplateId = templateId.Value;
        }

        return resultItems;
    }

    /// <summary>
    /// Метод создает новый статус шаблона пользователя учитывая ассоциацию статуса.
    /// </summary>
    /// <param name="createTaskStatusInput">Входная модель.</param>
    /// <exception cref="AggregateException">Если входные параметры не прошли валидацию.</exception>
    [HttpPost]
    [Route("user-task-status")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task CreateUserTaskStatusTemplateAsync([FromBody] CreateTaskStatusInput createTaskStatusInput)
    {
        var validator = await new CreateTaskStatusValidator().ValidateAsync(createTaskStatusInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка получения статусов для создания нового статуса.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.CreateUserTaskStatusTemplateAsync(
            createTaskStatusInput.AssociationStatusSysName, createTaskStatusInput.StatusName,
            createTaskStatusInput.StatusDescription, createTaskStatusInput.ProjectId, GetUserName());
    }

    /// <summary>
    /// Метод получает доступные переходы в статусы задачи.
    /// </summary>
    /// <param name="availableTaskStatusTransitionInput">Входная модель.</param>
    /// <returns>Список доступных переходов.</returns>
    [HttpGet]
    [Route("available-task-status-transitions")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<AvailableTaskStatusTransitionOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<AvailableTaskStatusTransitionOutput>> GetAvailableTaskStatusTransitionsAsync(
        [FromQuery] AvailableTaskStatusTransitionInput availableTaskStatusTransitionInput)
    {
        var validator = await new GetAvailableTaskStatusTransitionValidator()
            .ValidateAsync(availableTaskStatusTransitionInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка получения возможных переходов статусов задачи.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetAvailableTaskStatusTransitionsAsync(
            availableTaskStatusTransitionInput.ProjectId, availableTaskStatusTransitionInput.ProjectTaskId,
            availableTaskStatusTransitionInput.TaskDetailType);

        return result;
    }

    /// <summary>
    /// Метод изменяет статус задачи.
    /// </summary>
    /// <param name="changeTaskStatusInput">Входная модель.</param>
    [HttpPatch]
    [Route("task-status")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task ChangeTaskStatusAsync([FromBody] ChangeTaskStatusInput changeTaskStatusInput)
    {
        var validator = await new ChangeTaskStatusValidator().ValidateAsync(changeTaskStatusInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка изменения статуса задачи.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.ChangeTaskStatusAsync(changeTaskStatusInput.ProjectId,
            changeTaskStatusInput.ChangeStatusId, changeTaskStatusInput.TaskId, changeTaskStatusInput.TaskDetailType,
            GetUserName());
    }

    /// <summary>
    /// Метод обновления описание задачи.
    /// </summary>
    /// <param name="changeTaskDetailsInput">Входная модель.</param>
    [HttpPatch]
    [Route("task-details")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task UpdateTaskDetailsAsync([FromBody] ChangeTaskDetailsInput changeTaskDetailsInput)
    {
        // Если пустое описание, то это не ошибка, просто игнорим это.
        if (string.IsNullOrEmpty(changeTaskDetailsInput.ChangedTaskDetails))
        {
            return;
        }
        
        var validator = await new BaseChangeTaskValidator().ValidateAsync(changeTaskDetailsInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка изменения описания задачи.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.UpdateTaskDetailsAsync(changeTaskDetailsInput.ProjectId,
            changeTaskDetailsInput.TaskId, changeTaskDetailsInput.ChangedTaskDetails, GetUserName());
    }
    
    /// <summary>
    /// Метод обновления название задачи.
    /// </summary>
    /// <param name="changeTaskNameInput">Входная модель.</param>
    [HttpPatch]
    [Route("task-name")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task UpdateTaskNameAsync([FromBody] ChangeTaskNameInput changeTaskNameInput)
    {
        // Если пустое название, то это не ошибка, просто игнорим это.
        if (string.IsNullOrEmpty(changeTaskNameInput.ChangedTaskName))
        {
            return;
        }
        
        var validator = await new BaseChangeTaskValidator().ValidateAsync(changeTaskNameInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка изменения названия задачи.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.UpdateTaskNameAsync(changeTaskNameInput.ProjectId,
            changeTaskNameInput.TaskId, changeTaskNameInput.ChangedTaskName, GetUserName());
    }

    /// <summary>
    /// Метод привязывает тег к задаче проекта.
    /// Выбор происходит из набора тегов проекта.
    /// </summary>
    /// <param name="projectTaskTagInput">Входная модель.</param>
    [HttpPatch]
    [Route("attach-task-tag")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task AttachTaskTagAsync([FromBody] ProjectTaskTagInput projectTaskTagInput)
    {
        var validator = await new ProjectTaskTagValidator().ValidateAsync(projectTaskTagInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка привязки тега к задаче.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.AttachTaskTagAsync(projectTaskTagInput.TagId, projectTaskTagInput.ProjectTaskId,
            projectTaskTagInput.ProjectId, GetUserName());
    }
    
    /// <summary>
    /// Метод отвязывает тег от задачи проекта.
    /// </summary>
    /// <param name="projectTaskTagInput">Входная модель.</param>
    [HttpPatch]
    [Route("detach-task-tag")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)] 
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task DetachTaskTagAsync([FromBody] ProjectTaskTagInput projectTaskTagInput)
    {
        var validator = await new ProjectTaskTagValidator().ValidateAsync(projectTaskTagInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка отвязки тега от задачи.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.DetachTaskTagAsync(projectTaskTagInput.TagId, projectTaskTagInput.ProjectTaskId,
            projectTaskTagInput.ProjectId, GetUserName());
    }

    /// <summary>
    /// Метод создает связь с задачей (в зависимости от типа связи, который передали).
    /// </summary>
    /// <param name="taskLinkInput">Входная модель.</param>
    [HttpPost]
    [Route("task-link")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)] 
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task CreateTaskLinkAsync([FromBody] TaskLinkInput taskLinkInput)
    {
        var validator = await new TaskLinkValidator().ValidateAsync(taskLinkInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException($"Ошибка создания связи задачи (тип связи: {taskLinkInput.LinkType}).",
                exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.CreateTaskLinkAsync(taskLinkInput, GetUserName());
    }

    /// <summary>
    /// Метод получает связи задачи (обычные связи).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список связей задачи.</returns>
    [HttpGet]
    [Route("task-link-default")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<GetTaskLinkOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)] 
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkDefaultAsync([FromQuery] long projectId,
        [FromQuery] string projectTaskId, [FromQuery] LinkTypeEnum linkType)
    {
        var validator = await new GetTaskLinkValidator().ValidateAsync((projectId, projectTaskId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка получения связей задачи (обычные связи).", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetTaskLinkDefaultAsync(projectId, projectTaskId, linkType);

        return result;
    }
    
    /// <summary>
    /// Метод получает связи задачи (родительские связи).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список связей задачи.</returns>
    [HttpGet]
    [Route("task-link-parent")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<GetTaskLinkOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)] 
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkParentAsync([FromQuery] long projectId,
        [FromQuery] string projectTaskId, [FromQuery] LinkTypeEnum linkType)
    {
        var validator = await new GetTaskLinkValidator().ValidateAsync((projectId, projectTaskId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка получения связей задачи (родительские связи).", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetTaskLinkParentAsync(projectId, projectTaskId, linkType);

        return result;
    }
    
    /// <summary>
    /// Метод получает связи задачи (дочерние связи).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список связей задачи.</returns>
    [HttpGet]
    [Route("task-link-child")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<GetTaskLinkOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)] 
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkChildAsync([FromQuery] long projectId,
        [FromQuery] string projectTaskId, [FromQuery] LinkTypeEnum linkType)
    {
        var validator = await new GetTaskLinkValidator().ValidateAsync((projectId, projectTaskId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка получения связей задачи (дочерние связи).", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetTaskLinkChildAsync(projectId, projectTaskId, linkType);

        return result;
    }
    
    /// <summary>
    /// Метод получает связи задачи (связи зависит от).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список связей задачи.</returns>
    [HttpGet]
    [Route("task-link-depend")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<GetTaskLinkOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)] 
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkDependAsync([FromQuery] long projectId,
        [FromQuery] string projectTaskId, [FromQuery] LinkTypeEnum linkType)
    {
        var validator = await new GetTaskLinkValidator().ValidateAsync((projectId, projectTaskId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка получения связей задачи (связи зависит от).", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetTaskLinkDependAsync(projectId, projectTaskId, linkType);

        return result;
    }
    
    /// <summary>
    /// Метод получает связи задачи (связи блокирующие).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список связей задачи.</returns>
    [HttpGet]
    [Route("task-link-blocked")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<GetTaskLinkOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)] 
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkBlockedAsync([FromQuery] long projectId,
        [FromQuery] string projectTaskId, [FromQuery] LinkTypeEnum linkType)
    {
        var validator = await new GetTaskLinkValidator().ValidateAsync((projectId, projectTaskId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка получения связей задачи (блокирующая связь).", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetTaskLinkBlockedAsync(projectId, projectTaskId, linkType);

        return result;
    }

    /// <summary>
    /// Метод получает задачи проекта, которые доступны для создания связи с текущей задачей (разных типов связей).
    /// Под текущей задачей понимается задача, которую просматривает пользователь.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список задач, доступных к созданию связи.</returns>
    [HttpGet]
    [Route("available-task-link")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<AvailableTaskLinkOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)] 
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<AvailableTaskLinkOutput>> GetAvailableTaskLinkAsync([FromQuery] long projectId,
        [FromQuery] LinkTypeEnum linkType)
    {
        var validator = await new AvailableTaskLinkValidator().ValidateAsync((projectId, linkType));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка получения доступных задач для создания связи.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetAvailableTaskLinkAsync(projectId, linkType);

        return result;
    }

    /// <summary>
    /// Метод получает список типов связей задач.
    /// </summary>
    /// <returns>Список типов связей задач.</returns>
    [HttpGet]
    [Route("link-types")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<KeyValuePair<string,string>>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)] 
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<KeyValuePair<string,string>>> GetLinkTypesAsync()
    {
        var result = await GetLinkTypeFactory.GetLinkTypesAsync();

        return await Task.FromResult(result);
    }

    /// <summary>
    /// Метод привязывает наблюдателя задачи.
    /// </summary>
    /// <param name="projectTaskWatcherInput">Входная модель.</param>
    [HttpPatch]
    [Route("attach-task-watcher")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)] 
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task AttachTaskWatcherAsync([FromBody] ProjectTaskWatcherInput projectTaskWatcherInput)
    {
        var validator = await new ProjectTaskWatcherValidator().ValidateAsync(projectTaskWatcherInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка привязки наблюдателя задачи.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.AttachTaskWatcherAsync(projectTaskWatcherInput.WatcherId,
            projectTaskWatcherInput.ProjectTaskId, projectTaskWatcherInput.ProjectId, GetUserName());
    }
    
    /// <summary>
    /// Метод отвязывает наблюдателя задачи.
    /// </summary>
    /// <param name="projectTaskWatcherInput">Входная модель.</param>
    [HttpPatch]
    [Route("detach-task-watcher")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)] 
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task DetachTaskWatcherAsync([FromBody] ProjectTaskWatcherInput projectTaskWatcherInput)
    {
        var validator = await new ProjectTaskWatcherValidator().ValidateAsync(projectTaskWatcherInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка отвязки наблюдателя задачи.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.DetachTaskWatcherAsync(projectTaskWatcherInput.WatcherId,
            projectTaskWatcherInput.ProjectTaskId, projectTaskWatcherInput.ProjectId, GetUserName());
    }

    /// <summary>
    /// Метод обновляет исполнителя задачи.
    /// </summary>
    /// <param name="projectTaskExecutorInput">Входная модель.</param>
    [HttpPatch]
    [Route("task-executor")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)] 
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task UpdateTaskExecutorAsync([FromBody] ProjectTaskExecutorInput projectTaskExecutorInput)
    {
        var validator = await new ProjectTaskExecutorValidator().ValidateAsync(projectTaskExecutorInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка изменения исполнителя задачи.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.UpdateTaskExecutorAsync(projectTaskExecutorInput.ExecutorId,
            projectTaskExecutorInput.ProjectTaskId, projectTaskExecutorInput.ProjectId, GetUserName());
    }

    /// <summary>
    /// Метод обновляет приоритет задачи.
    /// </summary>
    /// <param name="taskPriorityInput">Входная модель.</param>
    [HttpPatch]
    [Route("task-priority")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task UpdateTaskPriorityAsync([FromBody] TaskPriorityInput taskPriorityInput)
    {
        var validator = await new UpdateTaskPriorityValidator().ValidateAsync(taskPriorityInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка привязки тега к задаче.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.UpdateTaskPriorityAsync(taskPriorityInput.PriorityId,
            taskPriorityInput.ProjectTaskId, taskPriorityInput.ProjectId, GetUserName());
    }

    /// <summary>
    /// Метод разрывает связь определенного типа между задачами.
    /// </summary>
    /// <param name="removeTaskLinkInput">Входная модель.</param>
    [HttpDelete]
    [Route("task-link")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task RemoveTaskLinkAsync([FromQuery] RemoveTaskLinkInput removeTaskLinkInput)
    {
        var validator = await new RemoveTaskLinkValidator().ValidateAsync(removeTaskLinkInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException(
                $"Ошибка разрыва связи: {removeTaskLinkInput.LinkType} между задачами: " +
                $"{removeTaskLinkInput.CurrentTaskId} и {removeTaskLinkInput.RemovedLinkId}.",
                exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.RemoveTaskLinkAsync(removeTaskLinkInput.LinkType,
            removeTaskLinkInput.RemovedLinkId, removeTaskLinkInput.CurrentTaskId, removeTaskLinkInput.ProjectId,
            GetUserName());
    }

    /// <summary>
    /// Метод добавляет файлы к задаче.
    /// </summary>
    /// <param name="formCollection">Данные формы.</param>
    [HttpPost]
    [Route("upload-task-file")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task UploadFilesAsync([FromForm] IFormCollection formCollection)
    {
        var projectTaskFileInput = JsonConvert.DeserializeObject<ProjectTaskFileInput>(
            formCollection["projectTaskFileInput"]);
        
        await _projectManagmentService.UploadFilesAsync(formCollection.Files, GetUserName(),
            projectTaskFileInput!.ProjectId, projectTaskFileInput!.TaskId);
    }
    
    /// <summary>
    /// Метод получает файлы задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="taskDetailType">Тип детализации.</param>
    /// <returns>Файлы задачи.</returns>
    [HttpGet]
    [Route("task-files")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectTaskFileOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)] 
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectTaskFileOutput>> GetProjectTaskFilesAsync([FromQuery] long projectId,
        [FromQuery] string projectTaskId, [FromQuery] TaskDetailTypeEnum taskDetailType)
    {
        var validator = await new ProjectTaskFileValidator().ValidateAsync((projectId, projectTaskId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка получение файлов задачи проекта. " +
                                            $"ProjectId: {projectId}. " +
                                            $"ProjectTaskId: {projectTaskId}.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }
        
        var items = await _projectManagmentService.GetProjectTaskFilesAsync(projectId, projectTaskId, taskDetailType);
        var result = _mapper.Map<IEnumerable<ProjectTaskFileOutput>>(items);

        return result;
    }
    
    /// <summary>
    /// Метод скачивает файл задачи.
    /// </summary>
    /// <param name="documentId">Id документа.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <returns>Файл задачи.</returns>
    [HttpGet]
    [Route("download-task-file")]
    [ProducesResponseType(200, Type = typeof(FileContentResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<FileContentResult> DownloadTaskFileAsync([FromQuery] long documentId, [FromQuery] long projectId,
        [FromQuery] string projectTaskId)
    {
        var validator = await new ProjectTaskFileValidator().ValidateAsync((projectId, projectTaskId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка валидации при скачивании файла задачи проекта. " +
                                            $"ProjectId: {projectId}. " +
                                            $"ProjectTaskId: {projectTaskId}. " +
                                            $"DocumentId: {documentId}.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }
        
        var result = await _projectManagmentService.DownloadFileAsync(documentId, projectId, projectTaskId);

        return result;
    }

    /// <summary>
    /// Метод удаляет файл задачи.
    /// </summary>
    /// <param name="mongoDocumentId">Id документа в MongoId.</param>
    [HttpDelete]
    [Route("task-file")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task RemoveTaskFileAsync([FromQuery] string? mongoDocumentId)
    {
        if (string.IsNullOrEmpty(mongoDocumentId))
        {
            var ex = new InvalidOperationException("Ошибка валидации при удалении файла задачи проекта. " +
                                                   $"MongoDocumentId: {mongoDocumentId}");
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.RemoveTaskFileAsync(mongoDocumentId);
    }

    /// <summary>
    /// Метод создает комментарий задачи.
    /// </summary>
    /// <param name="taskCommentInput">Входная модель.</param>
    /// <exception cref="AggregateException">Если не прошли валидацию входных параметров.</exception>
    [HttpPost]
    [Route("task-comment")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task CreateTaskCommentAsync([FromBody] TaskCommentInput taskCommentInput)
    {
        var validator = await new TaskCommentValidator().ValidateAsync(taskCommentInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка создания комментария задачи. " +
                                            $"ProjectId: {taskCommentInput.ProjectId}. " +
                                            $"ProjectTaskId: {taskCommentInput.ProjectTaskId}. " +
                                            $"Comment: {taskCommentInput.Comment}.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.CreateTaskCommentAsync(taskCommentInput.ProjectTaskId,
            taskCommentInput.ProjectId, taskCommentInput.Comment, GetUserName());
    }

    /// <summary>
    /// Метод получает список комментариев задачи.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список комментариев задачи.</returns>
    [HttpGet]
    [Route("task-comment")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TaskCommentOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<TaskCommentOutput>> GetTaskCommentsAsync([FromQuery] string projectTaskId,
        [FromQuery] long projectId)
    {
        var result = await _projectManagmentService.GetTaskCommentsAsync(projectTaskId, projectId);

        return result;
    }

    /// <summary>
    /// Метод обновляет комментарий задачи.
    /// </summary>
    /// <param name="taskCommentInput">Входная модель.</param>
    [HttpPut]
    [Route("task-comment")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task UpdateTaskCommentAsync([FromBody] TaskCommentExtendedInput taskCommentInput)
    {
        var validator = await new TaskCommentExtendedValidator().ValidateAsync(taskCommentInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка изменения комментария задачи. " +
                                            $"ProjectId: {taskCommentInput.ProjectId}. " +
                                            $"ProjectTaskId: {taskCommentInput.ProjectTaskId}. " +
                                            $"Comment: {taskCommentInput.Comment}." +
                                            $"CommentId: {taskCommentInput.CommentId}", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.UpdateTaskCommentAsync(taskCommentInput.ProjectTaskId,
            taskCommentInput.ProjectId, taskCommentInput.CommentId, taskCommentInput.Comment, GetUserName());
    }

    /// <summary>
    /// Метод удаляет комментарий задачи.
    /// </summary>
    /// <param name="commentId">Id комментария для удаления.</param>
    /// <exception cref="InvalidOperationException">Если входные данные невалидны.</exception>
    [HttpDelete]
    [Route("task-comment")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task DeleteTaskCommentAsync([FromQuery] long commentId)
    {
        if (commentId <= 0)
        {
            var ex = new InvalidOperationException($"Ошибка удаления комментария задачи. CommentId: {commentId}");
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.DeleteTaskCommentAsync(commentId, GetUserName());
    }

    /// <summary>
    /// Метод получает список эпиков.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список эпиков.</returns>
    [HttpGet]
    [Route("epics")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<EpicOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<EpicOutput>> GetEpicsAsync([FromQuery] long projectId)
    {
        if (projectId <= 0)
        {
            var ex = new InvalidOperationException($"Ошибка получение эпиков проекта. ProjectId: {projectId}");
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var items = await _projectManagmentService.GetEpicsAsync(projectId);
        var result = _mapper.Map<IEnumerable<EpicOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод получает список задач, историй для бэклога.
    /// Исключаются задачи в статусах: В архиве, готово, решена и тд.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список задач для бэклога.</returns>
    [HttpGet]
    [Route("backlog-tasks")]
    [ProducesResponseType(200, Type = typeof(ProjectManagmentWorkspaceResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectManagmentWorkspaceResult> GetBacklogTasksAsync([FromQuery] long projectId)
    {
        if (projectId <= 0)
        {
            var ex = new InvalidOperationException($"Ошибка получения задач бэклога проекта. ProjectId: {projectId}");
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetBacklogTasksAsync(projectId, GetUserName());

        return result;
    }

    /// <summary>
    /// Метод получает эпики, доступные к добавлению в них задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список эпиков.</returns>
    [HttpGet]
    [Route("available-epics")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<AvailableEpicOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<AvailableEpicOutput>> GetAvailableEpicsAsync([FromQuery] long projectId)
    {
        if (projectId <= 0)
        {
            var ex = new InvalidOperationException("Ошибка получения доступных эпиков для добавления задачи в эпик." +
                                                   $" ProjectId: {projectId}");
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var items = await _projectManagmentService.GetAvailableEpicsAsync(projectId);
        var result = _mapper.Map<IEnumerable<AvailableEpicOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод добавляет задачу в эпик.
    /// </summary>
    /// <param name="includeTaskEpicInput">Входная модель.</param>
    [HttpPost]
    [Route("task-epic")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task IncludeTaskEpicAsync([FromBody] IncludeTaskEpicInput includeTaskEpicInput)
    {
        var validator = await new TaskEpicValidator().ValidateAsync(includeTaskEpicInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка добавления задач в эпик. " +
                                            $"ProjectTaskIds: {includeTaskEpicInput.ProjectTaskIds}. " +
                                            $"EpicId: {includeTaskEpicInput.EpicId}", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.IncludeTaskEpicAsync(
            includeTaskEpicInput.EpicId.GetProjectTaskIdFromPrefixLink(), includeTaskEpicInput.ProjectTaskIds,
            GetUserName(), includeTaskEpicInput.ProjectId);
    }

    /// <summary>
    /// Метод получает список статусов истории для выбора.
    /// </summary>
    /// <returns>Список статусов истории.</returns>
    [HttpGet]
    [Route("history-statuses")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<UserStoryStatusOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<UserStoryStatusOutput>> GetUserStoryStatusesAsync()
    {
        var items = await _projectManagmentService.GetUserStoryStatusesAsync();
        
        // TODO: Не нужен маппинг будет когда заменим на DTO.
        var result = _mapper.Map<IEnumerable<UserStoryStatusOutput>>(items);

        return result;
    }
    
    /// <summary>
    /// Метод получает список статусов эпика для выбора.
    /// </summary>
    /// <returns>Список статусов эпика.</returns>
    [HttpGet]
    [Route("epic-statuses")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<UserStoryStatusOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<EpicStatusOutput>> GetEpicStatusesAsync()
    {
        var result = await _projectManagmentRepository.GetEpicStatusesAsync();

        return result;
    }

    /// <summary>
    /// Метод получает задачи эпика.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="epicId">Id эпика.</param>
    /// <returns>Список задач эпика.</returns>
    [HttpGet]
    [Route("epic-task")]
    [ProducesResponseType(200, Type = typeof(EpicTaskResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<EpicTaskResult> GetEpicTasksAsync([FromQuery] long projectId, [FromQuery] long epicId)
    {
        var validator = await new EpicTaskValidator().ValidateAsync((projectId, epicId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка получения задач эпика " +
                                            $"ProjectId: {projectId}. " +
                                            $"EpicId: {epicId}", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetEpicTasksAsync(projectId, epicId, GetUserName());

        return result;
    }

    /// <summary>
    /// Метод получает название спринта, в который входит задача.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <returns>Данные спринта.</returns>
    [HttpGet]
    [Route("task/sprint")]
    [ProducesResponseType(200, Type = typeof(TaskSprintOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<TaskSprintOutput> GetSprintTaskAsync([FromQuery] long projectId,
        [FromQuery] string projectTaskId)
    {
        var validator = await new SprintTaskValidator().ValidateAsync((projectId, projectTaskId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка получения спринта, в который входит задача. " +
                                            $"ProjectId: {projectId}. " +
                                            $"ProjectTaskId: {projectTaskId}", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetSprintTaskAsync(projectId, projectTaskId);

        return result;
    }

    /// <summary>
    /// Метод получает спринты, в которые может быть добавлена задача.
    /// Исключается спринт, в который задача уже добавлена.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <returns>Список спринтов, в которые может быть добавлена задача.</returns>
    [HttpGet]
    [Route("available-sprints")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TaskSprintOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<TaskSprintOutput>> GetAvailableProjectSprintsAsync([FromQuery] long projectId,
        [FromQuery] string projectTaskId)
    {
        var validator = await new SprintTaskValidator().ValidateAsync((projectId, projectTaskId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка получения спринтов, в которые может входить задача. " +
                                            $"ProjectId: {projectId}. " +
                                            $"ProjectTaskId: {projectTaskId}", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetAvailableProjectSprintsAsync(projectId, projectTaskId);

        return result;
    }

    /// <summary>
    /// Метод добавляет/обновляет спринт, в который входит задача.
    /// </summary>
    /// <param name="updateTaskSprintInput">Входная модель.</param>
    [HttpPut]
    [Route("task/sprint")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task InsertOrUpdateTaskSprintAsync([FromBody] UpdateTaskSprintInput updateTaskSprintInput)
    {
        var validator = await new UpdateTaskSprintValidator().ValidateAsync(updateTaskSprintInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка обновления спринта задачи. " +
                                            $"SprintId: {updateTaskSprintInput.SprintId}. " +
                                            $"ProjectTaskId: {updateTaskSprintInput.ProjectTaskId}", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.InsertOrUpdateTaskSprintAsync(updateTaskSprintInput.SprintId,
            updateTaskSprintInput.ProjectTaskId);
    }

    /// <summary>
    /// Метод получает все раб.пространства, в которых есть текущий пользователь.
    /// </summary>
    /// <returns>Список раб.пространств.</returns>
    [HttpGet]
    [Route("workspaces")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<WorkSpaceOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<WorkSpaceOutput>> GetWorkSpacesAsync()
    {
        var result = await _projectManagmentService.GetWorkSpacesAsync(GetUserName());

        return result;
    }
    
    /// <summary>
    /// Метод получает раб.пространство проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Раб.пространство проекта.</returns>
    [HttpGet]
    [Route("workspace")]
    [ProducesResponseType(200, Type = typeof(WorkSpaceOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<WorkSpaceOutput> GetWorkSpaceByProjectIdAsync([FromQuery] long projectId)
    {
        var result = await _projectManagmentService.GetWorkSpaceByProjectIdAsync(projectId, GetUserName());

        return result;
    }

    /// <summary>
    /// Метод удаляет задачу проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="taskType">Тип задачи.</param>
    [HttpDelete]
    [Route("task")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task RemoveProjectTaskAsync([FromQuery] long projectId, [FromQuery] string projectTaskId,
        [FromQuery] string taskType)
    {
        await _projectManagmentService.RemoveProjectTaskAsync(projectId, projectTaskId, GetUserName(),
            System.Enum.Parse<TaskDetailTypeEnum>(taskType));
    }
}