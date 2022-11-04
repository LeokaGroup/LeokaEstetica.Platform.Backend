using System.Data;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Models.Dto.Input.Project;
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
    /// </summary>
    [HttpGet]
    [Route("")]
    public async Task CatalogProjectsAsync()
    {
        
    }

    /// <summary>
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
        var result = await _projectService
            .CreateProjectAsync(createProjectInput.ProjectName, createProjectInput.ProjectDetails, GetUserName());

        return result;
    }

    [HttpGet]
    [Route("my")]
    [ProducesResponseType(200, Type = typeof(CreateProjectOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectResultOutput> MyProjectsAsync()
    {
        var result = new ProjectResultOutput();
        result.Projects.Columns.AddRange(new[]
        {
            new DataColumn
            {
                ColumnName = "Код проекта",
                DataType = typeof(string),
                DefaultValue = "Код проекта"
            },
            new DataColumn
            {
                ColumnName = "Название проекта",
                DataType = typeof(string),
                DefaultValue = "Название проекта"
            },
            new DataColumn
            {
                ColumnName = "Описание проекта",
                DataType = typeof(string),
                DefaultValue = "Описание проекта"
            }
        });

        return result;
    }
}