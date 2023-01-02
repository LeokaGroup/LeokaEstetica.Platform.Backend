using AutoMapper;
using FluentValidation;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Validators.Search;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Models.Dto.Input.Search.Project;
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

    public SearchController(IProjectFinderService projectFinderService, 
        IMapper mapper)
    {
        _projectFinderService = projectFinderService;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод ищет пользователей для приглашения в команду проекта.
    /// </summary>
    /// <param name="searchText">Поисковый запрос.</param>
    /// <returns>Список пользователей, которых можно пригласить в команду проекта.</returns>
    [HttpGet]
    [Route("project-members/{searchText}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<SearchProjectMemberOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<SearchProjectMemberOutput>> SearchInviteProjectMembersAsync(
        [FromRoute] SearchProjectMemberInput searchProjectMemberInput)
    {
        // Если поисковая строка невалидна, то не дергаем поиск.
        await new SearchInviteProjectMembersValidator().ValidateAndThrowAsync(searchProjectMemberInput);

        var items = await _projectFinderService.SearchInviteProjectMembersAsync(searchProjectMemberInput.SearchText);
        var result = _mapper.Map<IEnumerable<SearchProjectMemberOutput>>(items);
        
        return result;
    }
}