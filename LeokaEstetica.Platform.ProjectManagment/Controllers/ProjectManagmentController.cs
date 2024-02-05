using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Database.Abstractions.Template;
using LeokaEstetica.Platform.Integrations.Abstractions.Pachca;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.ProjectManagment.ValidationModels;
using LeokaEstetica.Platform.ProjectManagment.Validators;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Services.Abstractions.User;
using LeokaEstetica.Platform.Services.Factors;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.ProjectManagment.Controllers;

/// <summary>
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
    private readonly Lazy<IPachcaService> _pachcaService;
    private readonly Lazy<IProjectManagmentTemplateRepository> _projectManagmentTemplateRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectService">Сервис проектов пользователей.</param>
    /// <param name="projectManagmentService">Сервис управления проектами.</param>
    /// <param name="mapper">Маппер.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="userService">Сервис пользователей.</param>
    /// <param name="pachcaService">Сервис пачки.</param>
    /// <param name="projectManagmentTemplateRepository">Репозиторий шаблонов проектов.</param>
    public ProjectManagmentController(IProjectService projectService,
        IProjectManagmentService projectManagmentService,
        IMapper mapper,
        ILogger<ProjectManagmentController> logger,
        IUserService userService,
        Lazy<IPachcaService> pachcaService,
        Lazy<IProjectManagmentTemplateRepository> projectManagmentTemplateRepository)
    {
        _projectService = projectService;
        _projectManagmentService = projectManagmentService;
        _mapper = mapper;
        _logger = logger;
        _userService = userService;
        _pachcaService = pachcaService;
        _projectManagmentTemplateRepository = projectManagmentTemplateRepository;
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
    /// <returns>Данные конфигурации рабочего пространства.</returns>
    [HttpGet]
    [Route("config-workspace-template")]
    [ProducesResponseType(200, Type = typeof(ProjectManagmentWorkspaceResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectManagmentWorkspaceResult> GetConfigurationWorkSpaceBySelectedTemplateAsync(
        [FromQuery] long projectId)
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
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetConfigurationWorkSpaceBySelectedTemplateAsync(
            projectId, GetUserName());

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

        // TODO: Добавить отправку на фронт ивент на каждое сообщение через сокеты.
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
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
    public async Task<IEnumerable<ProjectTagOutput>> GetProjectTagsAsync()
    {
        var items = await _projectManagmentService.GetProjectTagsAsync();
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagmentService.GetAvailableTaskStatusTransitionsAsync(
            availableTaskStatusTransitionInput.ProjectId, availableTaskStatusTransitionInput.ProjectTaskId);

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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.ChangeTaskStatusAsync(changeTaskStatusInput.ProjectId,
            changeTaskStatusInput.ChangeStatusId, changeTaskStatusInput.TaskId);
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
        [FromQuery] long projectTaskId, [FromQuery] LinkTypeEnum linkType)
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
        [FromQuery] long projectTaskId, [FromQuery] LinkTypeEnum linkType)
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
        [FromQuery] long projectTaskId, [FromQuery] LinkTypeEnum linkType)
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
        [FromQuery] long projectTaskId, [FromQuery] LinkTypeEnum linkType)
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
        [FromQuery] long projectTaskId, [FromQuery] LinkTypeEnum linkType)
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
        var result = await GetLinkTypeFactory.GetLinkTypes();

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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
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
            
            await _pachcaService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.UpdateTaskPriorityAsync(taskPriorityInput.PriorityId,
            taskPriorityInput.ProjectTaskId, taskPriorityInput.ProjectId, GetUserName());
    }
}