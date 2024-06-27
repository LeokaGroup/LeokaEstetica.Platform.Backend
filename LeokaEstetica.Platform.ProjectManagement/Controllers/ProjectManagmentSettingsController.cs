using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Database.Abstractions.Template;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Input.Config;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.Notification;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;
using LeokaEstetica.Platform.ProjectManagment.ValidationModels;
using LeokaEstetica.Platform.ProjectManagment.Validators;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.ProjectManagement.Controllers;

/// <summary>
/// Контроллер настроек управления проектами.
/// </summary>
[ApiController]
[Route("project-management-settings")]
[AuthFilter]
public class ProjectManagmentSettingsController : BaseController
{
    private readonly ILogger<ProjectManagmentController> _logger;
    private readonly Lazy<IProjectManagmentTemplateRepository> _projectManagmentTemplateRepository;
    private readonly Lazy<IDiscordService> _discordService;
    private readonly IProjectManagmentService _projectManagmentService;
    private readonly IMapper _mapper;
    private readonly IProjectManagementSettingsService _projectManagementSettingsService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="projectManagmentTemplateRepository"></param>
    /// <param name="discordService">Сервис дискорда.</param>
    /// <param name="projectManagmentService"></param>
    /// <param name="mapper"></param>
    /// <param name="projectManagementSettingsService">Сервис настроек проекта.</param>
    public ProjectManagmentSettingsController(ILogger<ProjectManagmentController> logger,
        Lazy<IProjectManagmentTemplateRepository> projectManagmentTemplateRepository,
        Lazy<IDiscordService> discordService,
        IProjectManagmentService projectManagmentService,
         IMapper mapper,
          IProjectManagementSettingsService projectManagementSettingsService)
    {
        _logger = logger;
        _projectManagmentTemplateRepository = projectManagmentTemplateRepository;
        _discordService = discordService;
        _projectManagmentService = projectManagmentService;
        _mapper = mapper;
        _projectManagementSettingsService = projectManagementSettingsService;
    }
    
    /// <summary>
    /// TODO: Актуализировать название метода, название ендпоинта и тд.
    /// Метод создает метку (тег) проекта.
    /// </summary>
    /// <param name="projectTagInput">Входная модель.</param>
    [HttpPost]
    [Route("user-tag")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task CreateUserTaskTagAsync([FromBody] ProjectTagInput projectTagInput)
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
            
            var ex = new AggregateException("Ошибка создания статуса шаблона проекта.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.CreateUserTaskStatusTemplateAsync(
            createTaskStatusInput.AssociationStatusSysName, createTaskStatusInput.StatusName,
            createTaskStatusInput.StatusDescription, createTaskStatusInput.ProjectId, GetUserName());
    }

    // /// <summary>
    // /// TODO: Этот метод еще не доработан. Сделана лишь валидация.
    // /// Метод создает переход между статусами пользователя.
    // /// <param name="createTaskTransitionInput">Входная модель.</param>
    // /// </summary>
    // /// <exception cref="AggregateException">Если входные параметры не прошли валидацию.</exception>
    // [HttpPost]
    // [Route("user-transition")]
    // public async Task CreateUserTransitionAsync([FromBody] CreateTaskTransitionInput createTaskTransitionInput)
    // {
    //     var validator = await new CreateTaskTransitionValidator().ValidateAsync(createTaskTransitionInput);
    //
    //     if (validator.Errors.Any())
    //     {
    //         var exceptions = new List<InvalidOperationException>();
    //
    //         foreach (var err in validator.Errors)
    //         {
    //             exceptions.Add(new InvalidOperationException(err.ErrorMessage));
    //         }
    //         
    //         var ex = new AggregateException("Ошибка создания перехода между статусами пользователя.", exceptions);
    //         _logger.LogError(ex, ex.Message);
    //         
    //         await _discordService.Value.SendNotificationErrorAsync(ex);
    //         
    //         throw ex;
    //     }
    // }
    
    /// <summary>
    /// Метод фиксирует выбранную пользователем стратегию представления.
    /// </summary>
    /// <param name="fixationStrategyInput">Входная модель.</param>
    [HttpPatch]
    [Route("fixation-strategy")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task FixationProjectViewStrategyAsync(
        [FromBody] FixationProjectViewStrategyInput fixationStrategyInput)
    {
        var validator = await new FixationProjectViewStrategyValidator().ValidateAsync(fixationStrategyInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка фиксации стратегии представления пользователя.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.FixationProjectViewStrategyAsync(fixationStrategyInput.StrategySysName,
            fixationStrategyInput.ProjectId, GetUserName());
    }
    
    /// <summary>
    /// Метод скачивает файл изображения аватара пользователя.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Файл изображения аватара пользователя.</returns>
    [HttpGet]
    [Route("user-avatar-file")]
    [ProducesResponseType(200, Type = typeof(FileContentResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<FileContentResult> GetUserAvatarFileAsync([FromQuery] long projectId)
    {
        if (projectId <= 0)
        {
            var ex = new AggregateException("Ошибка валидации при скачивании файла изображения аватара пользователя. " +
                                            $"ProjectId: {projectId}.");
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }
        
        var result = await _projectManagmentService.GetUserAvatarFileAsync(projectId, GetUserName());

        return result;
    }

    /// <summary>
    /// Метод загружает новое изображение аватара пользователя.
    /// </summary>
    /// <param name="formCollection">Данные формы.</param>
    [HttpPost]
    [Route("user-avatar-file")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task UploadUserAvatarFileAsync([FromForm] IFormCollection formCollection)
    {
        if (!formCollection.Files.Any())
        {
            return;
        }
        
        var uploadUserAvatarInput = JsonConvert.DeserializeObject<UploadUserAvatarInput>(
            formCollection["uploadUserAvatarInput"]);

        await _projectManagmentService.UploadUserAvatarFileAsync(formCollection.Files, GetUserName(),
            uploadUserAvatarInput!.ProjectId);
    }

    /// <summary>
    /// Метод получает настройки длительности спринтов проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список настроек длительности спринтов проекта.</returns>
    [HttpGet]
    [Route("sprint-duration-settings")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<SprintDurationSetting>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<SprintDurationSetting>> GetProjectSprintsDurationSettingsAsync(
        [FromQuery] long projectId)
    {
        var result = await _projectManagementSettingsService.GetProjectSprintsDurationSettingsAsync(projectId,
            GetUserName());

        return result;
    }
    
    /// <summary>
    /// Метод получает настройки автоматического перемещения нерешенных задач спринта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список настройки автоматического перемещения нерешенных задач спринта.</returns>
    [HttpGet]
    [Route("sprint-move-not-completed-tasks-settings")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<SprintMoveNotCompletedTaskSetting>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<SprintMoveNotCompletedTaskSetting>>
        GetProjectSprintsMoveNotCompletedTasksSettingsAsync([FromQuery] long projectId)
    {
        var result = await _projectManagementSettingsService.GetProjectSprintsMoveNotCompletedTasksSettingsAsync(
            projectId, GetUserName());

        return result;
    }
    
    /// <summary>
    /// Метод обновляет настройки длительности спринтов проекта.
    /// </summary>
    /// <param name="sprintDurationSettingInput">Входная модель.</param>
    [HttpPatch]
    [Route("sprint-duration-settings")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task UpdateProjectSprintsDurationSettingsAsync(
        [FromBody] SprintDurationSettingInput sprintDurationSettingInput)
    {
        await _projectManagementSettingsService.UpdateProjectSprintsDurationSettingsAsync(
            sprintDurationSettingInput.ProjectId, sprintDurationSettingInput.IsSettingSelected, sprintDurationSettingInput.SysName);
    }
    
    /// <summary>
    /// Метод обновляет настройки перемещения нерешенных задач спринтов проекта.
    /// </summary>
    /// <param name="sprintMoveNotCompletedTaskSettingInput">Входная модель.</param>
    [HttpPatch]
    [Route("sprint-move-not-completed-tasks-settings")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task UpdateProjectSprintsMoveNotCompletedTasksSettingsAsync(
        [FromBody] SprintMoveNotCompletedTaskSettingInput sprintMoveNotCompletedTaskSettingInput)
    {
        await _projectManagementSettingsService.UpdateProjectSprintsMoveNotCompletedTasksSettingsAsync(
            sprintMoveNotCompletedTaskSettingInput.ProjectId,
            sprintMoveNotCompletedTaskSettingInput.IsSettingSelected,
            sprintMoveNotCompletedTaskSettingInput.SysName);
    }

    /// <summary>
    /// Метод получает список пользователей, которые состоят в проекте.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список пользователей.</returns>
    [HttpGet]
    [Route("company-project-users")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectSettingUserOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectSettingUserOutput>> GetCompanyProjectUsersAsync([FromQuery] long projectId)
    {
        if (projectId <= 0)
        {
            var ex = new AggregateException("Ошибка при получении пользователей проекта компании. " +
                                            $"ProjectId: {projectId}.");
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagementSettingsService.GetCompanyProjectUsersAsync(projectId, GetUserName());

        return result;
    }
    
    /// <summary>
    /// Метод получает список приглашений в проект.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список приглашений в проект.</returns>
    [HttpGet]
    [Route("project-invites")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectInviteOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectInviteOutput>> GetProjectInvitesAsync([FromQuery] long projectId)
    {
        if (projectId <= 0)
        {
            var ex = new AggregateException("Ошибка при получении приглашений проекта. " +
                                            $"ProjectId: {projectId}.");
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _projectManagementSettingsService.GetProjectInvitesAsync(projectId);

        return result;
    }
}