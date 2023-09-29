using AutoMapper;
using LeokaEstetica.Platform.Access.Abstractions.Resume;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Finder.Abstractions.Resume;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using LeokaEstetica.Platform.Services.Abstractions.Resume;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Resume;

/// <summary>
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
    private readonly IResumePaginationService _resumePaginationService;
    private readonly IAccessResumeService _accessResumeService;
    private readonly IFillColorResumeService _fillColorResumeService;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IFareRuleRepository _fareRuleRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="resumeService">Сервис резюме.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="resumeFinderService">Поисковый сервис резюме.</param>
    /// <param name="resumePaginationService">Сервис пагинации резюме.</param>
    /// <param name="accessResumeService">Сервис проверки доступа к базе резюме.</param>
    /// <param name="fillColorResumeService">Сервис выделения цветами анкет.</param>
    /// <param name="subscriptionRepository">Репозиторий подписок пользователя.</param>
    /// <param name="fareRuleRepository">Репозиторий правил тарифа.</param>
    public ResumeController(IResumeService resumeService, 
        IMapper mapper, 
        IResumeFinderService resumeFinderService, 
        IResumePaginationService resumePaginationService, 
        IAccessResumeService accessResumeService,
        IFillColorResumeService fillColorResumeService,
        ISubscriptionRepository subscriptionRepository,
        IFareRuleRepository fareRuleRepository)
    {
        _resumeService = resumeService;
        _mapper = mapper;
        _resumeFinderService = resumeFinderService;
        _resumePaginationService = resumePaginationService;
        _accessResumeService = accessResumeService;
        _fillColorResumeService = fillColorResumeService;
        _subscriptionRepository = subscriptionRepository;
        _fareRuleRepository = fareRuleRepository;
    }

    /// <summary>
    /// Метод получает список резюме.
    /// </summary>
    /// <returns>Список резюме.</returns>
    [HttpGet]
    [Route("catalog")]
    [ProducesResponseType(200, Type = typeof(ResumeResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ResumeResultOutput> GetProfileInfosAsync()
    {
        var result = await _resumeService.GetProfileInfosAsync();

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
    
    /// <summary>
    /// Метод пагинации резюме.
    /// </summary>
    /// <param name="page">Номер страницы.</param>
    /// <returns>Список резюме.</returns>
    [HttpGet]
    [Route("pagination/{page}")]
    [ProducesResponseType(200, Type = typeof(PaginationResumeOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<PaginationResumeOutput> GetResumesPaginationAsync([FromRoute] int page)
    {
        var result = await _resumePaginationService.GetResumesPaginationAsync(page);

        result.Resumes = await _fillColorResumeService.SetColorBusinessResume(result.Resumes, _subscriptionRepository,
            _fareRuleRepository) as List<ResumeOutput>;
        
        // Записываем анкетам коды пользователей.
        result.Resumes = await _resumeService.SetUserCodesAsync(result.Resumes) as List<ResumeOutput>;
            
        // Записываем теги анкетам.
        result.Resumes = await _resumeService.SetVacanciesTagsAsync(result.Resumes) as List<ResumeOutput>;

        return result;
    }

    /// <summary>
    /// Метод получает анкету пользователя по ее Id.
    /// </summary>
    /// <param name="resumeId">Id анкеты пользователя.</param>
    /// <returns>Данные анкеты.</returns>
    [HttpGet]
    [Route("{resumeId}")]
    [ProducesResponseType(200, Type = typeof(ResumeOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ResumeOutput> GetResumeAsync([FromRoute] long resumeId)
    {
        var result = await _resumeService.GetResumeAsync(resumeId);

        return result;
    }

    /// <summary>
    /// Метод проверяет доступ к базе резюме.
    /// </summary>
    /// <returns>Доступ.</returns>
    [HttpGet]
    [Route("access")]
    [ProducesResponseType(200, Type = typeof(AcessResumeOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<AcessResumeOutput> CheckAvailableAccessResumesAsync()
    {
        var result = await _accessResumeService.CheckAvailableResumesAsync(GetUserName());

        return result;
    }
    
    /// <summary>
    /// Метод получает список замечаний анкеты, если они есть.
    /// </summary>
    /// <returns>Список замечаний анкеты.</returns>
    [HttpGet]
    [Route("remarks")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<GetResumeRemarkOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<GetResumeRemarkOutput>> GetResumeRemarksAsync()
    {
        var items = await _resumeService.GetResumeRemarksAsync(GetUserName());
        var result = _mapper.Map<IEnumerable<GetResumeRemarkOutput>>(items);

        return result;
    }
}