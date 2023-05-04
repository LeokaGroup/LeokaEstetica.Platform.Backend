using AutoMapper;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Resume;
using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Services.Abstractions.Resume;

namespace LeokaEstetica.Platform.Services.Services.Resume;

/// <summary>
/// Класс реализует методы сервиса базы резюме.
/// </summary>
public class ResumeService : IResumeService
{
    private readonly ILogService _logService;
    private readonly IResumeRepository _resumeRepository;
    private readonly IMapper _mapper;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IFareRuleRepository _fareRuleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFillColorResumeService _fillColorResumeService;
    private readonly IResumeModerationRepository _resumeModerationRepository;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logService">Сервис логов.</param>
    /// <param name="resumeRepository">Репозиторий базы резюме.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="subscriptionRepository">Репозиторий подписок.</param>
    /// <param name="fareRuleRepository">Репозиторий правил тарифа.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="fillColorResumeService">Сервис выделение цветом резюме пользователей.</param>
    /// <param name="resumeModerationRepository">Репозиторий модерации анкет.</param>
    public ResumeService(ILogService logService, 
        IResumeRepository resumeRepository, 
        IMapper mapper, 
        ISubscriptionRepository subscriptionRepository, 
        IFareRuleRepository fareRuleRepository, 
        IUserRepository userRepository,
        IFillColorResumeService fillColorResumeService, 
        IResumeModerationRepository resumeModerationRepository)
    {
        _logService = logService;
        _resumeRepository = resumeRepository;
        _mapper = mapper;
        _subscriptionRepository = subscriptionRepository;
        _fareRuleRepository = fareRuleRepository;
        _userRepository = userRepository;
        _fillColorResumeService = fillColorResumeService;
        _resumeModerationRepository = resumeModerationRepository;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод возвращает список резюме.
    /// </summary>
    /// <returns>Список резюме.</returns>
    public async Task<ResumeResultOutput> GetProfileInfosAsync()
    {
        try
        {
            var profiles = await _resumeRepository.GetProfileInfosAsync();

            var result = new ResumeResultOutput
            {
                // Приводим к выходной модели.
                CatalogResumes = _mapper.Map<IEnumerable<ResumeOutput>>(profiles)
            };
            
            if (!result.CatalogResumes.Any())
            {
                return result;
            }
            
            result.CatalogResumes = await _fillColorResumeService.SetColorBusinessResume(result.CatalogResumes.ToList(),
                _subscriptionRepository, _fareRuleRepository);
            
            result.CatalogResumes = await SetUserCodes(result.CatalogResumes.ToList());
            
            result.CatalogResumes = await SetVacanciesTags(result.CatalogResumes.ToList());

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает анкету пользователя.
    /// </summary>
    /// <param name="resumeId">Id анкеты пользователя.</param>
    /// <returns>Данные анкеты.</returns>
    public async Task<ProfileInfoEntity> GetResumeAsync(long resumeId)
    {
        try
        {
            var result = await _resumeRepository.GetResumeAsync(resumeId);

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список замечаний анкеты, если они есть.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список замечаний анкеты.</returns>
    public async Task<IEnumerable<ResumeRemarkEntity>> GetResumeRemarksAsync(string account)
    {
        var userId = await _userRepository.GetUserIdByEmailAsync(account);
            
        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        var profileInfoId = await _userRepository.GetProfileInfoIdByUserIdAsync(userId);
        
        if (profileInfoId <= 0)
        {
            var ex = new InvalidOperationException("Ошибка при получении замечаний анкеты. " +
                                                   $"ProfileInfoId: {profileInfoId}");
            throw ex;
        }

        var isProjectOwner = await _resumeRepository.CheckResumeOwnerAsync(profileInfoId, userId);

        if (!isProjectOwner)
        {
            return Enumerable.Empty<ResumeRemarkEntity>();
        }

        var result = await _resumeModerationRepository.GetResumeRemarksAsync(profileInfoId);

        return result;
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод записывает коды пользователей.
    /// </summary>
    /// <param name="resumes">Список анкет пользователей.</param>
    /// <returns>Результирующий список.</returns>
    private async Task<IEnumerable<ResumeOutput>> SetUserCodes(List<ResumeOutput> resumes)
    {
        // Получаем словарь пользователей для поиска кодов, чтобы получить скорость поиска O(1).
        var userCodesDict = await _userRepository.GetUsersCodesAsync();
        
        foreach (var r in resumes)
        {
            if (userCodesDict.TryGetValue(r.UserId, out var code))
            {
                r.UserCode = code;   
            }
        }

        return resumes;
    }
    
    /// <summary>
    /// Метод проставляет флаги вакансиям пользователя в зависимости от его подписки.
    /// </summary>
    /// <param name="vacancies">Список вакансий каталога.</param>
    /// <returns>Список вакансий каталога с проставленными тегами.</returns>
    private async Task<IEnumerable<ResumeOutput>> SetVacanciesTags(List<ResumeOutput> vacancies)
    {
        foreach (var v in vacancies)
        {
            // Получаем подписку пользователя.
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(v.UserId);

            // Такая подписка не дает тегов.
            if (userSubscription.ObjectId < 3)
            {
                continue;
            }
            
            // Если подписка бизнес.
            if (userSubscription.ObjectId == 3)
            {
                v.TagColor = "warning";
                v.TagValue = "Бизнес";
            }
        
            // Если подписка профессиональный.
            if (userSubscription.ObjectId == 4)
            {
                v.TagColor = "warning";
                v.TagValue = "Профессиональный";
            }   
        }

        return vacancies;
    }

    #endregion
}
