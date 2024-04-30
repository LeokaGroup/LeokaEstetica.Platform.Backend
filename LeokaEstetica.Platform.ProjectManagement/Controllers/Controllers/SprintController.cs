using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
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
    
    /// <summary>
    /// Конструктор.
    /// <param name="sprintService">Сервис спринтов.</param>
    /// </summary>
    public SprintController(ISprintService sprintService)
    {
        _sprintService = sprintService;
    }

    /// <summary>
    /// Метод получает список спринтов для бэклога проекта.
    /// </summary>
    /// <param name="projectId"></param>
    [HttpGet]
    [Route("backlog-sprints")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task GetSprintsAsync([FromQuery] long projectId)
    {
        
    }
}