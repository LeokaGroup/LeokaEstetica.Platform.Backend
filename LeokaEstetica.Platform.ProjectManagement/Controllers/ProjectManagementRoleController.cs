using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.ProjectManagement.Controllers;

/// <summary>
/// Контроллер ролей управления проектами.
/// </summary>
[ApiController]
[Route("project-management-role")]
[AuthFilter]
public class ProjectManagementRoleController : BaseController
{
    private readonly IProjectManagmentRoleService _projectManagmentRoleService;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectManagmentRoleService">Сервис ролей модуля УП.</param>
    public ProjectManagementRoleController(IProjectManagmentRoleService projectManagmentRoleService)
    {
        _projectManagmentRoleService = projectManagmentRoleService;
    }

    /// <summary>
    /// Метод получает список ролей пользователя.
    /// </summary>
    /// <param name="projectId">Id проекта, если передали.</param>
    /// <returns>Список ролей.</returns>
    [HttpGet]
    [Route("user-roles")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectManagementRoleOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectManagementRoleOutput>> GetUserRolesAsync([FromQuery] long? projectId = null)
    {
        
    }
}