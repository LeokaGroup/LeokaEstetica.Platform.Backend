using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Finder.Abstractions.Resume;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using LeokaEstetica.Platform.Services.Abstractions.Resume;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Resume;

/// <summary>
/// TODO: Доступ к этому модулю только для пользователей, которые приобрели подписку.
/// Контроллер работы с резюме.
/// </summary>
[AuthFilter]
[ApiController]
[Route("resumes")]
public class ResumeController : BaseController
{
    private readonly IResumeService _resumeService;
    private readonly IMapper _mapper;
    private readonly IResumeFinderService _resumeFinderService;
    
    public ResumeController(IResumeService resumeService, 
        IMapper mapper, 
        IResumeFinderService resumeFinderService)
    {
        _resumeService = resumeService;
        _mapper = mapper;
        _resumeFinderService = resumeFinderService;
    }

    /// <summary>
    /// Метод получает список резюме.
    /// </summary>
    /// <returns>Список резюме.</returns>
    [HttpGet]
    [Route("")]
    [ProducesResponseType(200, Type = typeof(ResumeResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ResumeResultOutput> GetProfileInfosAsync()
    {
        var result = new ResumeResultOutput();
        var items = await _resumeService.GetProfileInfosAsync();
        result.CatalogResumes = _mapper.Map<IEnumerable<ResumeOutput>>(items);

        return result;
    }

    /// <summary>
    /// Метод находит резюме по поисковому запросу.
    /// </summary>
    /// <param name="searchText">Поисковая строка.</param>
    /// <returns>Список резюме после поиска.</returns>
    [HttpGet]
    [Route("search")]
    [ProducesResponseType(200, Type = typeof(ResumeResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ResumeResultOutput> SearchResumesAsync([FromQuery] string searchText)
    {
        var result = await _resumeFinderService.SearchResumesAsync(searchText);

        return result;
    }
}