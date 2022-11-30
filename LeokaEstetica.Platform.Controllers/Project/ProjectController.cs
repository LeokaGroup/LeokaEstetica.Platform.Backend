using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Project;

/// <summary>
/// Контроллер работы с проектами.
/// </summary>
[AuthFilter]
[ApiController]
[Route("projects")]
public class ProjectController : BaseController
{
    private readonly IProjectService _projectService;
    
    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// TODO: Подумать, давать ли всем пользователям возможность просматривать каталог проектов или только тем, у кого есть подписка.
    /// Метод получает список проектов для каталога.
    /// </summary>
    /// <returns>Список проектов.</returns>
    [HttpGet]
    [Route("")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<CatalogProjectOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<CatalogProjectOutput>> CatalogProjectsAsync()
    {
        var result = await _projectService.CatalogProjectsAsync();

        return result;
    }

    /// <summary>
    /// TODO: Нужно также учитывать иконку проекта. Пока она не передается в БД.
    /// Метод создает новый проект пользователя.
    /// </summary>
    /// <param name="createProjectInput">Входная модель.</param>
    /// <returns>Данные нового проекта.</returns>
    [HttpPost]
    [Route("project")]
    [ProducesResponseType(200, Type = typeof(CreateProjectOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CreateProjectOutput> CreateProjectAsync([FromBody] CreateProjectInput createProjectInput)
    {
        var result = await _projectService.CreateProjectAsync(createProjectInput.ProjectName, createProjectInput.ProjectDetails, GetUserName(), createProjectInput.ProjectStage);

        return result;
    }

    /// <summary>
    /// Метод получает список проектов пользователя.
    /// </summary>
    /// <returns>Список проектов.</returns>
    [HttpGet]
    [Route("user-projects")]
    [ProducesResponseType(200, Type = typeof(UserProjectResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UserProjectResultOutput> UserProjectsAsync()
    {
        var result = await _projectService.UserProjectsAsync(GetUserName());

        return result;
    }

    /// <summary>
    /// Метод получает названия полей для таблицы проектов пользователя.
    /// Все названия столбцов этой таблицы одинаковые у всех пользователей.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    [HttpGet]
    [Route("config-user-projects")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectColumnNameOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectColumnNameOutput>> UserProjectsColumnsNamesAsync()
    {
        var result = await _projectService.UserProjectsColumnsNamesAsync();

        return result;
    }

    /// <summary>
    /// Метод обновляет проект.
    /// </summary>
    /// <param name="createProjectInput">Входная модель.</param>
    /// <returns>Обновленные данные.</returns>
    [HttpPut]
    [Route("project")]
    [ProducesResponseType(200, Type = typeof(UpdateProjectOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UpdateProjectOutput> UpdateProjectAsync([FromBody] UpdateProjectInput createProjectInput)
    {
        var result = await _projectService.UpdateProjectAsync(createProjectInput.ProjectName, createProjectInput.ProjectDetails, GetUserName(), createProjectInput.ProjectId, createProjectInput.ProjectStage);

        return result;
    }

    /// <summary>
    /// Метод получает проект для изменения или просмотра.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="mode">Режим. Чтение или изменение.</param>
    /// <returns>Данные проекта.</returns>
    [HttpGet]
    [Route("project")]
    [ProducesResponseType(200, Type = typeof(ProjectOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectOutput> GetProjectAsync([FromQuery] long projectId, [FromQuery] ModeEnum mode)
    {
        var result = await _projectService.GetProjectAsync(projectId, mode, GetUserName());

        return result;
    }

    /// <summary>
    /// Метод получает стадии проекта для выбора.
    /// </summary>
    /// <returns>Стадии проекта.</returns>
    [HttpGet]
    [Route("stages")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectStageOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectStageOutput>> ProjectStagesAsync()
    {
        var result = await _projectService.ProjectStagesAsync();

        return result;
    }

    /// <summary>
    /// Метод получает список вакансий проекта. Список вакансий, которые принадлежат владельцу проекта.
    /// </summary>
    /// <param name="projectId">Id проекта, вакансии которого нужно получить.</param>
    /// <returns>Список вакансий.</returns>
    [HttpGet]
    [Route("vacancies")]
    [ProducesResponseType(200, Type = typeof(ProjectVacancyResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectVacancyResultOutput> ProjectVacanciesAsync([FromQuery] long projectId)
    {
        var result = await _projectService.ProjectVacanciesAsync(projectId);

        return result;
    }
}