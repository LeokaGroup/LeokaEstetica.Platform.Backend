using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.ProjectManagment.Controllers;

/// <summary>
/// Контроллер управления проектами.
/// </summary>
[ApiController]
[Route("project-managment")]
[AuthFilter]
public class ProjectManagmentController : BaseController
{
    private readonly IProjectService _projectService;
    private readonly IProjectManagmentService _projectManagmentService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectService">Сервис проектов пользователей.</param>
    /// <param name="projectManagmentService">Сервис управления проектами.</param>
    /// <param name="mapper">Маппер.</param>
    public ProjectManagmentController(IProjectService projectService,
        IProjectManagmentService projectManagmentService,
        IMapper mapper)
    {
        _projectService = projectService;
        _projectManagmentService = projectManagmentService;
        _mapper = mapper;
    }

    /// <summary>
    /// TODO: Подумать, стоит ли выводить в рабочее пространство архивные проекты и те, что находятся на модерации.
    /// Метод получает список проектов пользователя.
    /// </summary>
    /// <returns>Список проектов пользователя.</returns>
    [HttpGet]
    [Route("user-projects")]
    [ProducesResponseType(200, Type = typeof(UserProjectResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UserProjectResultOutput> UserProjectsAsync()
    {
        var result = await _projectService.UserProjectsAsync(GetUserName(), false);

        return result;
    }

    /// <summary>
    /// Метод получает список стратегий представления рабочего пространства.
    /// </summary>
    /// <returns>Список стратегий.</returns>
    [HttpGet]
    [Route("view-strategies")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ViewStrategyOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ViewStrategyOutput>> GetViewStrategiesAsync()
    {
        var items = await _projectManagmentService.GetViewStrategiesAsync();
        var result = _mapper.Map<IEnumerable<ViewStrategyOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод получает элементы верхнего меню (хидера).
    /// </summary>
    /// <returns>Список элементов.</returns>
    [HttpGet]
    [Route("header")]
    [ProducesResponseType(200, Type = typeof(List<ProjectManagmentHeaderResult>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<List<ProjectManagmentHeaderResult>> GetHeaderItemsAsync()
    {
        // Получаем необработанные списки. Доп.списки пока не заполнены.
        var unprocessedItems = await _projectManagmentService.GetHeaderItemsAsync();
        var mapItems = _mapper.Map<IEnumerable<ProjectManagmentHeaderOutput>>(unprocessedItems);
        
        // Наполняем доп.списки.
        var result = await _projectManagmentService.ModifyHeaderItemsAsync(mapItems);

        return result;
    }

    /// <summary>
    /// Метод получает список шаблонов задач, которые пользователь может выбрать перед переходом в рабочее пространство.
    /// </summary>
    /// <returns>Список шаблонов задач.</returns>
    [HttpGet]
    [Route("templates")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectManagmentTaskTemplateResult>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProjectManagmentTaskTemplateResult>> GetProjectManagmentTemplatesAsync()
    {
        var items = await _projectManagmentService.GetProjectManagmentTemplatesAsync();
        var result = _mapper.Map<IEnumerable<ProjectManagmentTaskTemplateResult>>(items);
        var resultItems = result.ToList();
        
        // Проставляем Id шаблона статусам.
        await _projectManagmentService.SetProjectManagmentTemplateIdsAsync(resultItems);

        return resultItems;
    }
}