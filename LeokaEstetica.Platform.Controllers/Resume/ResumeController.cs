using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using LeokaEstetica.Platform.Services.Abstractions.Resume;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Resume;

/// <summary>
/// TODO: Доступ к этому модулю только для пользователей, которые приобрели подписку.
/// </summary>
[AuthFilter]
[ApiController]
[Route("resumes")]
public class ResumeController : BaseController
{
    private readonly IResumeService _resumeService;
    private readonly IMapper _mapper;
    
    public ResumeController(IResumeService resumeService, 
        IMapper mapper)
    {
        _resumeService = resumeService;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод получает список резюме.
    /// </summary>
    /// <returns>Список резюме.</returns>
    [HttpGet]
    [Route("")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ResumeOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ResumeOutput>> GetProfileInfosAsync()
    {
        var items = await _resumeService.GetProfileInfosAsync();
        var result = _mapper.Map<IEnumerable<ResumeOutput>>(items);

        return result;
    }
}