using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.ProjectManagement.Controllers.Controllers;

/// <summary>
/// Контроллер работы со спринтами.
/// </summary>
[ApiController]
[Route("sprint")]
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
    [Route("backlog-sprints")]
    [ProducesResponseType(200)]
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
}