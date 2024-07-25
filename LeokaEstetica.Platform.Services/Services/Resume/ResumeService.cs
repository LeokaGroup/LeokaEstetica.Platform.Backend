using System.Data;
using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Resume;
using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Services.Abstractions.Resume;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.Resume;

/// <summary>
/// Класс реализует методы сервиса базы резюме.
/// </summary>
internal sealed class ResumeService : IResumeService
{
    private readonly ILogger<ResumeService> _logger;
    private readonly IResumeRepository _resumeRepository;
    private readonly IMapper _mapper;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IFareRuleRepository _fareRuleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFillColorResumeService _fillColorResumeService;
    private readonly IResumeModerationRepository _resumeModerationRepository;
    private readonly IAccessUserService _accessUserService;
    private readonly IDiscordService _discordService;
    
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
    /// <param name="accessUserService">Сервис проверки доступа.</param>
    /// <param name="pachcaService">Сервис пачки.</param>
    public ResumeService(ILogger<ResumeService> logger, 
        IResumeRepository resumeRepository, 
        IMapper mapper, 
        ISubscriptionRepository subscriptionRepository, 
        IFareRuleRepository fareRuleRepository, 
        IUserRepository userRepository,
        IFillColorResumeService fillColorResumeService, 
        IResumeModerationRepository resumeModerationRepository,
        IAccessUserService accessUserService,
        IDiscordService discordService)
    {
        _logger = logger;
        _resumeRepository = resumeRepository;
        _mapper = mapper;
        _subscriptionRepository = subscriptionRepository;
        _fareRuleRepository = fareRuleRepository;
        _userRepository = userRepository;
        _fillColorResumeService = fillColorResumeService;
        _resumeModerationRepository = resumeModerationRepository;
        _accessUserService = accessUserService;
        _discordService = discordService;
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

            var removedProfile = new List<ProfileInfoEntity>();
            
            // Исключаем анкеты, которые не проходят по условиям.
            foreach (var p in profiles)
            {
                var isAccess = await _accessUserService.IsProfileEmptyAsync(p.UserId);
                var isRemarks = await _resumeModerationRepository.GetResumeRemarksAsync(p.ProfileInfoId);
                
                if (isAccess || isRemarks.Any())
                {
                    removedProfile.Add(p);
                }
            }

            // Удаляем из списка те анкеты, которые не прошли по условиям.
            if (removedProfile.Any())
            {
                profiles.RemoveAll(p => removedProfile.Select(x => x.ProfileInfoId).Contains(p.ProfileInfoId));
            }

            var result = new ResumeResultOutput
            {
                // Приводим к выходной модели.
                CatalogResumes = _mapper.Map<IEnumerable<UserInfoOutput>>(profiles)
            };
            
            if (!result.CatalogResumes.Any())
            {
                return result;
            }
            
            result.CatalogResumes = await _fillColorResumeService.SetColorBusinessResume(result.CatalogResumes.ToList(),
                _subscriptionRepository, _fareRuleRepository);
            
            result.CatalogResumes = await SetUserCodesAsync(result.CatalogResumes.ToList());
            
            result.CatalogResumes = await SetVacanciesTagsAsync(result.CatalogResumes.ToList());

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает анкету пользователя по ее Id.
    /// </summary>
    /// <param name="resumeId">Id анкеты пользователя.</param>
    /// <returns>Данные анкеты.</returns>
    public async Task<UserInfoOutput> GetResumeAsync(long resumeId, string account)
    {
        try
        {
            //Проверяем наличие резюме
            var result = await _resumeRepository.GetResumeAsync(resumeId);

            if (result == null)
            {
                return new UserInfoOutput
                {
                    IsAccess = false,
                    IsSuccess = false,
                };
            }

            //Проверяем наличие пользователя
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            //Проверяем доступ к анкете
            var isOwner = await _resumeRepository.CheckResumeOwnerAsync(result.ProfileInfoId, userId);

            var resumeModeration = await _resumeModerationRepository.GetResumeModerationByProfileInfosIdsAsync(result.ProfileInfoId);

			//проверка, чтобы анкета не была на модерации -если ссылку вбил не владелец анкеты

			if (!isOwner && resumeModeration != null &&
                (resumeModeration.ModerationStatusId == (int)ResumeModerationStatusEnum.ModerationResume)) 
            {
                return new UserInfoOutput
                {
                    IsAccess=false,
                    IsSuccess=false,
                };

			}

            // Наполняем результат.
            await CreateModifyUserResult(result);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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
    public async Task<IEnumerable<UserInfoOutput>> SetUserCodesAsync(List<UserInfoOutput> resumes)
    {
        // Получаем словарь пользователей для поиска кодов, чтобы получить скорость поиска O(1).
        var userCodesDict = await _userRepository.GetUsersCodesAsync();
        
        foreach (var r in resumes)
        {
            r.UserCode = userCodesDict.TryGet(r.UserId);   
        }

        return resumes;
    }
    
    /// <summary>
    /// Метод проставляет флаги вакансиям пользователя в зависимости от его подписки.
    /// </summary>
    /// <param name="vacancies">Список вакансий каталога.</param>
    /// <returns>Список вакансий каталога с проставленными тегами.</returns>
    public async Task<IEnumerable<UserInfoOutput>> SetVacanciesTagsAsync(List<UserInfoOutput> vacancies)
    {
        foreach (var v in vacancies)
        {
            var userId = v.UserId;

            // Получаем подписку пользователя.
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(userId);

            if (userSubscription is null)
            {
                var ex = new InvalidOperationException("Найдена невалидная подписка пользователя. " +
                                                    $"UserId: {userId}. " +
                                                    "Подписка была NULL или невалидная." +
                                                    $"Ошибка в {nameof(ResumeService)}");
                // Отправляем ивент в пачку.
                await _discordService.SendNotificationErrorAsync(ex);
                
                // Если ошибка, то не стопаем выполнение логики, а вернем вакансии, пока будем разбираться с ошибкой.
                // Без тегов не страшно отобразить вакансии.
                return vacancies;
            }

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

    /// <summary>
    /// Метод наполняет результат о пользователе доп.данными.
    /// </summary>
    /// <param name="result">Результат до наполнения.</param>
    /// <exception cref="InvalidOperationException">Если не удалось получить доп.данные о пользователе.</exception>
    private async Task CreateModifyUserResult(UserInfoOutput result)
    {
        var userId = result.UserId;
        var modifyUser = await _userRepository.GetUserPhoneEmailByUserIdAsync(userId);

        if (modifyUser is null)
        {
            throw new InvalidOperationException("Не удалось получить дополнительную информацию о пользователе." +
                                                $"UserId: {userId}");
        }

        result.Email = modifyUser.Email;
        result.PhoneNumber = modifyUser.PhoneNumber;
    }

    #endregion
}
