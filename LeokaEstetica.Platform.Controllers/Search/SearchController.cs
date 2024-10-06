using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Models.Dto.Output.Search.Project;
using LeokaEstetica.Platform.Services.Abstractions.Search.Project;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Search;

/// <summary>
/// Контроллер содержит всю логику поиска.
/// </summary>
[AuthFilter]
[ApiController]
[Route("search")]
public class SearchController : BaseController
{
    private readonly IProjectFinderService _projectFinderService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectFinderService">Сервис поиска в проектах.</param>
    /// <param name="mapper">Автомаппер.</param>
    public SearchController(IProjectFinderService projectFinderService, 
        IMapper mapper)
    {
        _projectFinderService = projectFinderService;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод ищет пользователей для приглашения в команду проекта.
    /// </summary>
    /// <param name="searchText">Поисковый текст.</param>
    /// <returns>Список пользователей, которых можно пригласить в команду проекта.</returns>
    [HttpGet]
    [Route("project-members")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<SearchProjectMemberOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<SearchProjectMemberOutput>> SearchInviteProjectMembersAsync(
        [FromQuery] string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return Enumerable.Empty<SearchProjectMemberOutput>();
        }

        var items = await _projectFinderService.SearchInviteProjectMembersAsync(searchText, GetTokenFromHeader());
        
        // TODO: Мапить сразу в выходной модели и через Dapper.
        var result = _mapper.Map<IEnumerable<SearchProjectMemberOutput>>(items);
        
        return result;
    }

    /// <summary>
    /// Метод ищет пользователей по их почте.
    /// </summary>
    /// <param name="searchText">Поисковый текст.</param>
    /// <returns>Список пользователей, которых можно пригласить в команду проекта.</returns>
    [HttpGet]
    [Route("search-user")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<SearchProjectMemberOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<SearchProjectMemberOutput> SearchUserByEmailAsync([FromQuery] string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return new SearchProjectMemberOutput();
        }

        var items = await _projectFinderService.SearchUserByEmailAsync(searchText, GetUserName());

        // TODO: Мапить сразу в выходной модели и через Dapper.
        var result = _mapper.Map<SearchProjectMemberOutput>(items);

        return result;
    }
}