using AutoMapper;
using FluentValidation;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Controllers.Validators.Search;
using LeokaEstetica.Platform.Models.Dto.Input.Search.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Profile;
using LeokaEstetica.Platform.Models.Dto.Output.Search.Project;
using LeokaEstetica.Platform.Services.Abstractions.Search.Profile;
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
    private readonly ISearchProfileService _searchProfileService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectFinderService">Сервис поиска в проектах.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="searchProfileService">Сервис поиска в профиле пользователя.</param>
    public SearchController(IProjectFinderService projectFinderService, 
        IMapper mapper,
        ISearchProfileService searchProfileService)
    {
        _projectFinderService = projectFinderService;
        _mapper = mapper;
        _searchProfileService = searchProfileService;
    }

    /// <summary>
    /// Метод ищет пользователей для приглашения в команду проекта.
    /// </summary>
    /// <param name="searchProjectMemberInput">Входная модель.</param>
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

        var items = await _projectFinderService.SearchInviteProjectMembersAsync(searchProjectMemberInput.SearchText,
            GetTokenFromHeader());
        
        var result = _mapper.Map<IEnumerable<SearchProjectMemberOutput>>(items);
        
        return result;
    }

    /// <summary>
    /// Метод поиска навыков по названию навыка.
    /// </summary>
    /// <param name="searchText">Поисковый текст.</param>
    /// <returns>Список навыков, которые удалось найти.</returns>
    [HttpGet]
    [Route("skill")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<SkillOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<SkillOutput>> SearchSkillsByNameAsync([FromQuery] string searchText)
    {
        var items = await _searchProfileService.SearchSkillsByNameAsync(searchText.ToLower());
        var result = _mapper.Map<IEnumerable<SkillOutput>>(items);

        return result;
    }
}