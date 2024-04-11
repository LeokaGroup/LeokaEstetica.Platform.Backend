using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Database.Abstractions.Template;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Input.Config;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.ProjectManagment.ValidationModels;
using LeokaEstetica.Platform.ProjectManagment.Validators;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.ProjectManagment.Controllers;

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

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="projectManagmentTemplateRepository"></param>
    /// <param name="discordService">Сервис дискорда.</param>
    /// <param name="projectManagmentService"></param>
    /// <param name="mapper"></param>
    public ProjectManagmentSettingsController(ILogger<ProjectManagmentController> logger,
        Lazy<IProjectManagmentTemplateRepository> projectManagmentTemplateRepository,
        Lazy<IDiscordService> discordService,
        IProjectManagmentService projectManagmentService,
         IMapper mapper)
    {
        _logger = logger;
        _projectManagmentTemplateRepository = projectManagmentTemplateRepository;
        _discordService = discordService;
        _projectManagmentService = projectManagmentService;
        _mapper = mapper;
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
}