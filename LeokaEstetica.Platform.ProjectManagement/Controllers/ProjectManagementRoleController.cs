using Dapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;
using LeokaEstetica.Platform.ProjectManagement.Validators;
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
    private readonly Lazy<IProjectManagmentRoleRepository> _projectManagmentRoleRepository;
    private readonly ILogger<ProjectManagementRoleController> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectManagmentRoleService">Сервис ролей модуля УП.</param>
    /// <param name="projectManagmentRoleRepository">Репозиторий ролей пользователей.</param>
    /// <param name="logger">Логгер.</param>
    public ProjectManagementRoleController(IProjectManagmentRoleService projectManagmentRoleService, Lazy<IProjectManagmentRoleRepository> projectManagmentRoleRepository,
     ILogger<ProjectManagementRoleController> logger)
    {
        _projectManagmentRoleService = projectManagmentRoleService;
        _projectManagmentRoleRepository = projectManagmentRoleRepository;
        _logger = logger;
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
        var result = await _projectManagmentRoleService.GetUserRolesAsync(GetUserName(), projectId);

        return result;
    }
    
    /// <summary>
    /// Метод получает список ролей.
    /// </summary>
    /// <param name="projectId">Id проекта, если передали.</param>
    /// <returns>Список ролей.</returns>
    [HttpGet]
    [Route("roles")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectManagementRoleOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectManagementRoleOutput>> GetRolesAsync([FromQuery] long? projectId = null)
    {
        var result = await _projectManagmentRoleRepository.Value.GetUserRolesAsync(null, projectId);

        if (result is null)
        {
            return Enumerable.Empty<ProjectManagementRoleOutput>();
        }

        return result;
    }

    /// <summary>
    /// Метод обновляет роли пользователей.
    /// </summary>
    /// <param name="roles">Список ролей к обновлению.</param>
    [HttpPut]
    [Route("roles")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task UpdateRolesAsync([FromBody] IEnumerable<ProjectManagementRoleInput> roles)
    {
        var updatedRoles = roles.AsList();
        var validator = await new ProjectManagementRoleValidator().ValidateAsync(updatedRoles);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();
            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException(exceptions);
            _logger.LogError(ex, "Ошибки при обновлении ролей.");

            throw ex;
        }

        await _projectManagmentRoleService.UpdateRolesAsync(updatedRoles, CreateTokenFromHeader());
    }
}