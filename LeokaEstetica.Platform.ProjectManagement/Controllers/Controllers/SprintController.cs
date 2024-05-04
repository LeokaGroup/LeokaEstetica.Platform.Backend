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

    /// <summary>
    /// Конструктор.
    /// <param name="sprintService">Сервис спринтов.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="logger">Логгер.</param>
    /// </summary>
    public SprintController(ISprintService sprintService,
        Lazy<IDiscordService> discordService,
        ILogger<SprintController> logger)
    {
        _sprintService = sprintService;
        _discordService = discordService;
        _logger = logger;
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

            var ex = new AggregateException("Ошибка при обновлении название спринта. " +
                                            $"ProjectSprintId: {updateSprintInput.ProjectSprintId}. " +
                                            $"ProjectId: {updateSprintInput.ProjectId}.", exceptions);
            _logger.LogError(ex, ex.Message);
            
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        await _sprintService.UpdateSprintNameAsync(updateSprintInput.ProjectSprintId, updateSprintInput.ProjectId,
            updateSprintInput.SprintName, GetUserName());
    }
}