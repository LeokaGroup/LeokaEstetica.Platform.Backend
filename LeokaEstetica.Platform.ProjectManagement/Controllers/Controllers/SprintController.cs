using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.ProjectManagement.Validators;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.ProjectManagement.Controllers.Controllers;

/// <summary>
/// Контроллер работы со спринтами.
/// </summary>
[ApiController]
[Route("project-management/sprints")]
[AuthFilter]
public class SprintController : BaseController
{
    private readonly ISprintService _sprintService;
    private readonly Lazy<IDiscordService> _discordService;
    private readonly ILogger<SprintController> _logger;
    private readonly IProjectManagmentService _projectManagmentService;

    /// <summary>
    /// Конструктор.
    /// <param name="sprintService">Сервис спринтов.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="projectManagmentService">Сервис управления проектами.</param>
    /// </summary>
    public SprintController(ISprintService sprintService,
        Lazy<IDiscordService> discordService,
        ILogger<SprintController> logger,
        IProjectManagmentService projectManagmentService)
    {
        _sprintService = sprintService;
        _discordService = discordService;
        _logger = logger;
        _projectManagmentService = projectManagmentService;
    }

    /// <summary>
    /// Метод получает список спринтов для бэклога проекта.
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns>Список спринтов бэклога проекта.</returns>
    [HttpGet]
    [Route("sprint-list")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TaskSprintExtendedOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<TaskSprintExtendedOutput>> GetSprintsAsync([FromQuery] long projectId)
    {
        if (projectId <= 0)
        {
            var ex = new InvalidOperationException("Ошибка при получении спринтов бэклога проекта. " +
                                                   $"ProjectId: {projectId}");
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            // TODO: Тут добавить уведомление через хаб для отображения на фронте.
            
            return Enumerable.Empty<TaskSprintExtendedOutput>();
        }

        var result = await _sprintService.GetSprintsAsync(projectId);

        return result;
    }

    /// <summary>
    /// Метод получает детали спринта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Детали спринта.</returns>
    [HttpGet]
    [Route("sprint")]
    [ProducesResponseType(200, Type = typeof(TaskSprintExtendedOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<TaskSprintExtendedOutput> GetSprintAsync([FromQuery] long projectSprintId,
        [FromQuery] long projectId)
    {
        var validator = await new SprintValidator().ValidateAsync((projectSprintId, projectId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка получения деталей спринта. " +
                                            $"ProjectSprintId: {projectSprintId}. " +
                                            $"ProjectId: {projectId}.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _sprintService.GetSprintAsync(projectSprintId, projectId, GetUserName());

        return result;
    }
    
    /// <summary>
    /// Метод планирует спринт.
    /// Добавляет задачи в спринт, если их указали при планировании спринта.
    /// </summary>
    /// <param name="planingSprintInput">Входная модель.</param>
    [HttpPost]
    [Route("sprint/planing")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task PlaningSprintAsync([FromBody] PlaningSprintInput planingSprintInput)
    {
        var validator = await new PlaningSprintValidator().ValidateAsync(planingSprintInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка планирования спринта. " +
                                            $"ProjectId: {planingSprintInput.ProjectId}. " +
                                            $"SprintName: {planingSprintInput.SprintName}", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _projectManagmentService.PlaningSprintAsync(planingSprintInput, GetUserName(), CreateTokenFromHeader());
    }

    /// <summary>
    /// Метод обновляет название спринта.
    /// </summary>
    /// <param name="updateSprintInput">Входная модель.</param>
    [HttpPatch]
    [Route("sprint-name")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task UpdateSprintNameAsync([FromBody] UpdateSprintNameInput updateSprintInput)
    {
        var validator = await new UpdateSprintNameValidator().ValidateAsync(updateSprintInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка при обновлении описания спринта. " +
                                            $"ProjectSprintId: {updateSprintInput.ProjectSprintId}. " +
                                            $"ProjectId: {updateSprintInput.ProjectId}.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _sprintService.UpdateSprintNameAsync(updateSprintInput.ProjectSprintId, updateSprintInput.ProjectId,
            updateSprintInput.SprintName, GetUserName());
    }
    
    /// <summary>
    /// Метод обновляет описание спринта.
    /// </summary>
    /// <param name="updateSprintInput">Входная модель.</param>
    [HttpPatch]
    [Route("sprint-details")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task UpdateSprintDetailsAsync([FromBody] UpdateSprintDetailsInput updateSprintInput)
    {
        var validator = await new UpdateSprintDetailsValidator().ValidateAsync(updateSprintInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка при обновлении описания спринта. " +
                                            $"ProjectSprintId: {updateSprintInput.ProjectSprintId}. " +
                                            $"ProjectId: {updateSprintInput.ProjectId}.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _sprintService.UpdateSprintDetailsAsync(updateSprintInput.ProjectSprintId, updateSprintInput.ProjectId,
            updateSprintInput.SprintDetails, GetUserName());
    }
    
    /// <summary>
    /// Метод проставляет/обновляет исполнителя спринта (ответственный за выполнение спринта).
    /// </summary>
    /// <param name="insertOrUpdateSprintExecutorInput">Входная модель.</param>
    [HttpPatch]
    [Route("sprint-executor")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task InsertOrUpdateSprintExecutorAsync(
        [FromBody] InsertOrUpdateSprintExecutorInput insertOrUpdateSprintExecutorInput)
    {
        var validator = await new InsertOrUpdateSprintExecutorValidator()
            .ValidateAsync(insertOrUpdateSprintExecutorInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка при проставлении/обновлении исполнителя спринта. " +
                                            $"ProjectSprintId: {insertOrUpdateSprintExecutorInput.ProjectSprintId}. " +
                                            $"ProjectId: {insertOrUpdateSprintExecutorInput.ProjectId}. " +
                                            $"ExecutorId: {insertOrUpdateSprintExecutorInput.ExecutorId}.",
                exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _sprintService.InsertOrUpdateSprintExecutorAsync(insertOrUpdateSprintExecutorInput.ProjectSprintId,
            insertOrUpdateSprintExecutorInput.ProjectId, insertOrUpdateSprintExecutorInput.ExecutorId, GetUserName());
    }
    
    /// <summary>
    /// Метод проставляет/обновляет наблюдателей спринта.
    /// </summary>
    /// <param name="insertOrUpdateSprintWatchersInput">Входная модель.</param>
    [HttpPatch]
    [Route("sprint-watcher")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task InsertOrUpdateSprintWatchersAsync(
        [FromBody] InsertOrUpdateSprintWatchersInput insertOrUpdateSprintWatchersInput)
    {
        var validator = await new InsertOrUpdateSprintWatchersValidator()
            .ValidateAsync(insertOrUpdateSprintWatchersInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка при проставлении/обновлении наблюдателей спринта. " +
                                            $"ProjectSprintId: {insertOrUpdateSprintWatchersInput.ProjectSprintId}. " +
                                            $"ProjectId: {insertOrUpdateSprintWatchersInput.ProjectId}. " +
                                            $"WatcherIds: {insertOrUpdateSprintWatchersInput.WatcherIds}.",
                exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _sprintService.InsertOrUpdateSprintWatchersAsync(insertOrUpdateSprintWatchersInput.ProjectSprintId,
            insertOrUpdateSprintWatchersInput.ProjectId, insertOrUpdateSprintWatchersInput.WatcherIds, GetUserName());
    }

    /// <summary>
    /// Метод начинает спринт.
    /// Перед началом спринта проводится ряд проверок.
    /// </summary>
    /// <param name="sprintInput">Входная модель.</param>
    [HttpPatch]
    [Route("sprint/start")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task StartSprintAsync([FromBody] SprintInput sprintInput)
    {
        var validator = await new SprintValidator()
            .ValidateAsync((sprintInput.ProjectSprintId, sprintInput.ProjectId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка начала спринта. " +
                                            $"ProjectSprintId: {sprintInput.ProjectSprintId}. " +
                                            $"ProjectId: {sprintInput.ProjectId}.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _sprintService.StartSprintAsync(sprintInput.ProjectSprintId, sprintInput.ProjectId, GetUserName(),
            CreateTokenFromHeader());
    }

    /// <summary>
    /// Метод завершает спринт (ручное завершение).
    /// </summary>
    /// <param name="sprintInput">Входная модель.</param>
    [HttpPatch]
    [Route("sprint/manual-complete")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ManualCompleteSprintOutput> ManualCompleteSprintAsync(
        [FromBody] ManualCompleteSprintInput sprintInput)
    {
        var validator = await new SprintValidator()
            .ValidateAsync((sprintInput.ProjectSprintId, sprintInput.ProjectId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка завершения спринта (вручную). " +
                                            $"ProjectSprintId: {sprintInput.ProjectSprintId}. " +
                                            $"ProjectId: {sprintInput.ProjectId}.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _sprintService.ManualCompleteSprintAsync(sprintInput, GetUserName(),
            CreateTokenFromHeader());

        return result;
    }

    /// <summary>
    /// Метод получает список спринтов доступных для переноса незавершенных задач в один из них.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список спринтов.</returns>
    [HttpGet]
    [Route("available-next-sprints")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TaskSprintExtendedOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<TaskSprintExtendedOutput>> GetAvailableNextSprintsAsync(
        [FromQuery] long projectSprintId, [FromQuery] long projectId)
    {
        var validator = await new SprintValidator().ValidateAsync((projectSprintId, projectId));

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException("Ошибка получения будущих спринтов. " +
                                            $"ProjectSprintId: {projectSprintId}. " +
                                            $"ProjectId: {projectId}.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _sprintService.GetAvailableNextSprintsAsync(projectSprintId, projectId);

        return result;
    }
}