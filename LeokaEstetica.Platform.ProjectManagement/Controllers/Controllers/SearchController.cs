using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Input.Search.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Search.ProjectManagement;
using LeokaEstetica.Platform.ProjectManagment.Validators;
using LeokaEstetica.Platform.Services.Abstractions.Search.ProjectManagment;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.ProjectManagment.Controllers;

/// <summary>
/// Контроллер поиска в модуле управления проектами.
/// </summary>
[ApiController]
[Route("project-management-search")]
[AuthFilter]
public class SearchController : BaseController
{
    private readonly ILogger<SearchController> _logger;
    private readonly Lazy<IDiscordService> _discordService;
    private readonly ISearchProjectManagementService _searchProjectManagementService;

    /// <summary>
    /// Контроллер.
    /// <param name="logger">Логгер.</param>
    /// <param name="discordService">Сервис дискорда.</param>
    /// <param name="searchProjectManagementService">Сервис поиска в модуле УП.</param>
    /// </summary>
    public SearchController(ILogger<SearchController> logger,
     Lazy<IDiscordService> discordService,
     ISearchProjectManagementService searchProjectManagementService)
    {
        _logger = logger;
        _discordService = discordService;
        _searchProjectManagementService = searchProjectManagementService;
    }

    /// <summary>
    /// Метод поиска задач.
    /// Поиск происходит по атрибутам, которые передали.
    /// </summary>
    /// <param name="searchTaskInput">Входная модель.</param>
    /// <returns>Список найденных задач.</returns>
    [HttpGet]
    [Route("search-task")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<SearchAgileObjectOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<SearchAgileObjectOutput>> SearchTaskAsync([FromQuery] SearchTaskInput searchTaskInput)
    {
        if (string.IsNullOrWhiteSpace(searchTaskInput.SearchText))
        {
            return Enumerable.Empty<SearchAgileObjectOutput>();
        }
        
        var validator = await new SearchTaskValidator().ValidateAsync(searchTaskInput);

        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }
            
            var ex = new AggregateException("Ошибка поиска задач.", exceptions);
            _logger.LogError(ex, ex.Message);
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            throw ex;
        }

        var result = await _searchProjectManagementService.SearchTaskAsync(searchTaskInput.SearchText,
            searchTaskInput.ProjectIds, searchTaskInput.IsById, searchTaskInput.IsByName,
            searchTaskInput.IsByDescription);

        return result;
    }

    /// <summary>
    /// Метод находит Agile-объекты. Это может быть задача, эпик, история, ошибка, спринт.
    /// </summary>
    /// <param name="searchText">Поисковый текст./</param>
    /// <param name="isSearchByProjectTaskId">Признак поиска по Id задачи в рамках проекта.</param>
    /// <param name="isSearchByTaskName">Признак поиска по названию задачи.</param>
    /// <param name="isSearchByTaskDescription">Признак поиска по совпадению в описании.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Найденные задачи, истории, эпики, ошибки.</returns>
    [HttpGet]
    [Route("search-agile-object")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<SearchAgileObjectOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<SearchAgileObjectOutput>> SearchAgileObjectAsync([FromQuery] string searchText,
        [FromQuery] bool isSearchByProjectTaskId, [FromQuery] bool isSearchByTaskName,
        [FromQuery] bool isSearchByTaskDescription, [FromQuery] long projectId)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return Enumerable.Empty<SearchAgileObjectOutput>();
        }

        var result = await _searchProjectManagementService.SearchAgileObjectAsync(searchText.Trim(),
            isSearchByProjectTaskId, isSearchByTaskName, isSearchByTaskDescription, projectId, GetUserName());

        return result;
    }
}