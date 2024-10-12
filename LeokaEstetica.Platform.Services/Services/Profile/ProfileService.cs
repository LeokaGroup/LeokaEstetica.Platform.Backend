using System.Runtime.CompilerServices;
using AutoMapper;
using Dapper;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Resume;
using LeokaEstetica.Platform.Database.Abstractions.Profile;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Input.Profile;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;
using LeokaEstetica.Platform.Models.Dto.Output.Profile;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Redis.Abstractions.Profile;
using LeokaEstetica.Platform.Redis.Models.Profile;
using LeokaEstetica.Platform.Services.Abstractions.Profile;
using Microsoft.Extensions.Logging;
using Items = LeokaEstetica.Platform.Models.Dto.Output.Profile.ProfileItems;
using ProfileItems = LeokaEstetica.Platform.Redis.Models.Profile.ProfileItems;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.Profile;

/// <summary>
/// Класс реализует методы сервиса профиля пользователя.
/// </summary>
internal sealed class ProfileService : IProfileService
{
    private readonly ILogger<ProfileService> _logger;
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IProfileRedisService _profileRedisService;
    private readonly IAccessUserService _accessUserService;
    private readonly IResumeModerationRepository _resumeModerationRepository;
    private readonly IDiscordService _discordService;
    private readonly Lazy<IHubNotificationService> _hubNotificationService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Сервис логов.</param>
    /// <param name="profileRepository">Репозиторий профиля пользователя.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="profileRedisService">Сервис кэша.</param>
    /// <param name="accessUserService">Сервис доступа пользователей.</param>
    /// <param name="resumeModerationRepository">Репозиторий модерации анкет.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="hubNotificationService">Сервис уведомлений хабов.</param>
    public ProfileService(ILogger<ProfileService> logger,
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        IMapper mapper,
        IProfileRedisService profileRedisService,
        IAccessUserService accessUserService,
        IResumeModerationRepository resumeModerationRepository,
        IDiscordService discordService,
        Lazy<IHubNotificationService> hubNotificationService)
    {
        _logger = logger;
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _profileRedisService = profileRedisService;
        _accessUserService = accessUserService;
        _resumeModerationRepository = resumeModerationRepository;
        _discordService = discordService;
        _hubNotificationService = hubNotificationService;
    }

    /// <summary>
    /// Метод получает основную информацию раздела обо мне.
    /// <param name="account">Аккаунт пользователя.</param>
    /// </summary>
    /// <returns>Данные раздела обо мне.</returns>
    public async Task<ProfileInfoOutput> GetProfileInfoAsync(string account)
    {
        try
        {
            if (string.IsNullOrEmpty(account))
            {
                throw new ArgumentException("Имя аккаунта не передано.");
            }

            // Ищем userId по VK Id если, передано числовое значение.
            if (long.TryParse(account, out long userId))
            {
                userId = await _userRepository.GetUserIdByVkIdAsync(userId);
                
                if (userId <= 0)
                {
                    throw new InvalidOperationException($"Пользователя с VK Id {userId} не существует в системе.");
                }
            }

            // Ищем userId по email, если передано строковое значение.
            if(userId == 0)
            {
                userId = await _userRepository.GetUserByEmailAsync(account);

                if (userId <= 0)
                {
                    throw new InvalidOperationException($"Пользователя с почтой {account} не существует в системе.");
                }
            }
            
            var profileInfo = await _profileRepository.GetProfileInfoAsync(userId);

            if (profileInfo is null)
            {
                throw new InvalidOperationException($"Не найдено информации о профиле пользователя с UserId: {userId}");
            }

            var result = _mapper.Map<ProfileInfoOutput>(profileInfo);

            // Получаем основную информацию профиля пользователя.
            var userData = await _userRepository.GetUserPhoneEmailByUserIdAsync(userId);

            if (userData is null)
            {
                return result;
            }

            result.Email = userData.Email;
            result.PhoneNumber = userData.PhoneNumber;

            var remarks = await _resumeModerationRepository.GetResumeRemarksAsync(profileInfo.ProfileInfoId);
            result.ResumeRemarks = _mapper.Map<IEnumerable<ResumeRemarkOutput>>(remarks);
            
            // Если выбрана сокращенная форма фамили, то отображаем лишь первую букву и точку.
            if (result.IsShortFirstName)
            {
                result.LastName = string.Concat(result.LastName.Substring(0, 1), ".");
            }
            
            // Проверяем заполнение анкеты и даем доступ либо нет.
            result.IsEmptyProfile = await _accessUserService.IsProfileEmptyAsync(userId);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список элементов меню профиля пользователя.
    /// </summary>
    /// <returns>Список элементов меню профиля пользователя.</returns>
    public async Task<ProfileMenuItemsResultOutput> ProfileMenuItemsAsync()
    {
        try
        {
            var items = await _profileRepository.ProfileMenuItemsAsync();
            var result = _mapper.Map<ProfileMenuItemsResultOutput>(items);

            // Добавляем меню профиля в кэш.
            var model = CreateFactoryModelToRedis(result.ProfileMenuItems);
            await _profileRedisService.SaveProfileMenuCacheAsync(model);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список навыков для выбора в профиль пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список навыков.</returns>
    public async Task<List<SkillOutput>> ProfileSkillsAsync(string account)
    {
        try
        {
            var items = await _profileRepository.ProfileSkillsAsync();
            var result = _mapper.Map<List<SkillOutput>>(items);
            
            // Исключаем те навыки, которые уже выбраны пользователем.
            var userSkills = await SelectedProfileUserSkillsAsync(null, account);

            // Находим Id навыков, которые ранее были выбраны пользователем.
            var ids = userSkills.Select(s => s.SkillId);
            result.RemoveAll(s => ids.Contains(s.SkillId));

            return result;;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список целей на платформе для выбора пользователем в профиль пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список целей.</returns>
    public async Task<List<IntentOutput>> ProfileIntentsAsync(string account)
    {
        try
        {
            var items = await _profileRepository.ProfileIntentsAsync();
            var result = _mapper.Map<List<IntentOutput>>(items);
            
            // Исключаем те цели, которые уже выбраны пользователем.
            var userIntents = await SelectedProfileUserIntentsAsync(null, account);

            // Находим Id целей, которые ранее были выбраны пользователем.
            var ids = userIntents.Select(i => i.IntentId);
            result.RemoveAll(s => ids.Contains(s.IntentId));

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод сохраняет данные анкеты пользователя.
    /// </summary>
    /// <param name="profileInfoInput">Входная модель.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Сохраненные данные.</returns>
    public async Task<ProfileInfoOutput> SaveProfileInfoAsync(ProfileInfoInput profileInfoInput, string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId == 0)
        {
            throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найдено.");
        }

        var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
            
        try
        {
            // Получаем данные профиля пользователя.
            var profileInfo = await _profileRepository.GetProfileInfoAsync(userId);

            if (profileInfo is null)
            {
                throw new InvalidOperationException($"Для пользователя с Id {userId} не заведено профиля в системе.");
            }
            
            // TODO: Вернуть, если решим вернуть этот функционал на фронте.
            // Оставляем исходную фамилию, если нет признака сокращения фамилии.
            // if (!profileInfoInput.IsShortFirstName)
            // {
            //     profileInfoInput.LastName = profileInfo.LastName;
            // }

            CreateProfileInfoModel(profileInfoInput, ref profileInfo);

            // Сохраняем данные пользователя.
            var savedProfileInfo = await _profileRepository.SaveProfileInfoAsync(profileInfo);
            var result = _mapper.Map<ProfileInfoOutput>(savedProfileInfo);
            result.PhoneNumber = profileInfoInput.PhoneNumber;

            var email = profileInfoInput.Email;

            // Сохраняем номер телефона пользователя.
            var savedProfileInfoData = await _userRepository.SaveUserDataAsync(userId, profileInfoInput.PhoneNumber,
                email!);

            if (profileInfoInput.UserSkills is not null && profileInfoInput.UserSkills.Any())
            {
                // Сохраняем выбранные навыки пользователя.
                await SaveUserSkillsAsync(profileInfoInput, userId);
            }
            
            if (profileInfoInput.UserIntents is not null && profileInfoInput.UserIntents.Any())
            {
                // Сохраняем выбранные цели пользователя.
                await SaveUserIntentsAsync(profileInfoInput, userId);
            }

            // Снова логиним юзера, так как почта изменилась а значит и токен надо менять.
            if (savedProfileInfoData.IsEmailChanged)
            {
                var claim = await _accessUserService.GetIdentityClaimAsync(email);
                var userToken = await _accessUserService.CreateTokenFactoryAsync(claim);   
                
                result.Email = email;
                result.Token = userToken;
            }

            var profileInfoId = profileInfo.ProfileInfoId;
            
            // Проверяем наличие неисправленных замечаний.
            await CheckAwaitingCorrectionRemarksAsync(profileInfoId);

            result.IsSuccess = true;
            result.IsEmailChanged = savedProfileInfoData.IsEmailChanged;

            // Отправляем анкету на модерацию.            
            await _resumeModerationRepository.AddResumeModerationAsync(profileInfoId);
            
            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо", "Данные успешно сохранены.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotifySuccessSaveProfileInfo", userCode,
                UserConnectionModuleEnum.Main);

            // Отправляем уведомление в пачку об изменениях анкеты.
            await _discordService.SendNotificationChangedProfileInfoBeforeModerationAsync(profileInfoId);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            
            await _hubNotificationService.Value.SendNotificationAsync("Что то не так...",
             "Ошибка при сохранении данных. Мы уже знаем о проблеме и разбираемся с ней.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotifyErrorSaveProfileInfo", userCode,
                UserConnectionModuleEnum.Main);
            
            throw;
        }
    }

    /// <summary>
    /// Метод сохраняет навыки пользователя.
    /// </summary>
    /// <param name="profileInfoInput">Входная модель.</param>
    /// <param name="userId">Id пользователя.</param>
    private async Task SaveUserSkillsAsync(ProfileInfoInput profileInfoInput, long userId)
    {
        await _profileRepository.SaveProfileSkillsAsync(profileInfoInput.UserSkills
            .Select(s => new UserSkillEntity
            {
                SkillId = s.SkillId,
                UserId = userId,
                Position = s.Position
            }), userId);
    }

    /// <summary>
    /// Метод сохраняет цели пользователя.
    /// </summary>
    /// <param name="profileInfoInput">Входная модель.</param>
    /// <param name="userId">Id пользователя.</param>
    private async Task SaveUserIntentsAsync(ProfileInfoInput profileInfoInput, long userId)
    {
        await _profileRepository.SaveProfileIntentsAsync(profileInfoInput.UserIntents
            .Select(s => new UserIntentEntity
            {
                IntentId = s.IntentId,
                UserId = userId,
                Position = s.Position
            }), userId);
    }

    /// <summary>
    /// Метод создает модель для сохранения данных профиля пользователя.
    /// </summary>
    /// <param name="profileInfoInput">Входная модель.</param>
    /// <param name="profileInfoId">Id профиля пользователя.</param>
    private void CreateProfileInfoModel(ProfileInfoInput profileInfoInput, ref ProfileInfoEntity profileInfo)
    {
        profileInfo.FirstName = profileInfoInput.FirstName;
        profileInfo.LastName = profileInfoInput.LastName;
        profileInfo.Patronymic = profileInfoInput.Patronymic;
        profileInfo.Aboutme = profileInfoInput.Aboutme;
        profileInfo.IsShortFirstName = profileInfoInput.IsShortFirstName;
        profileInfo.Job = profileInfoInput.Job;
        profileInfo.WhatsApp = profileInfoInput.WhatsApp;
        profileInfo.Telegram = profileInfoInput.Telegram;
        profileInfo.Vkontakte = profileInfoInput.Vkontakte;
        profileInfo.OtherLink = profileInfoInput.OtherLink;
        profileInfo.WorkExperience = profileInfoInput.WorkExperience;
    }

    /// <summary>
    /// Метод создает модель для сохранения в кэше Redis.
    /// </summary>
    /// <param name="items">Список меню.</param>
    /// <returns>Модель для сохранения.</returns>
    private ProfileMenuRedis CreateFactoryModelToRedis(IReadOnlyCollection<ProfileMenuItemsOutput> items)
    {
        var model = new ProfileMenuRedis
        {
            ProfileMenuItems = new List<ProfileMenuItemsRedis>(items.Select(pmi => new ProfileMenuItemsRedis
            {
                SysName = pmi.SysName,
                Label = pmi.Label,
                Url = pmi.Url,
                Items = pmi.Items.Select(i => new ProfileItems
                    {
                        SysName = i.SysName,
                        Label = i.Label,
                        Url = i.Url,
                    })
                    .ToList()
            }))
        };

        return model;
    }

    /// <summary>
    /// Метод создает модель получения из кэша Redis.
    /// </summary>
    /// <param name="items">Список меню.</param>
    /// <returns>Модель с результатами.</returns>
    private ProfileMenuRedis CreateFactoryModelFromRedis(IReadOnlyCollection<ProfileMenuItemsRedis> items)
    {
        var model = new ProfileMenuRedis
        {
            ProfileMenuItems = new List<ProfileMenuItemsRedis>(items.Select(pmi => new ProfileMenuItemsRedis
            {
                SysName = pmi.SysName,
                Label = pmi.Label,
                Url = pmi.Url,
                Items = pmi.Items.Select(i => new ProfileItems
                    {
                        SysName = i.SysName,
                        Label = i.Label,
                        Url = i.Url,
                    })
                    .AsList()
            }))
        };

        return model;
    }

    /// <summary>
    /// Метод выбирает пункт меню профиля пользователя. Производит действия, если нужны. 
    /// </summary>
    /// <param name="selectMenuInput">Входная модель.</param>
    /// <returns>Системное название действия и роут если нужно.</returns>
    public async Task<SelectMenuOutput> SelectProfileMenuAsync(string text)
    {
        try
        {
            var result = new SelectMenuOutput();

            // Ищем меню профиля в кэше.
            var redisData = await _profileRedisService.GetProfileMenuCacheAsync();

            // Если данные в кэше есть, берем оттуда.
            if (!redisData.ProfileMenuItems.Any())
            {
                // Данных в кэше не оказалось, подгружаем из БД, и снова заберем из кэша, так как данные там уже будут.
                await ProfileMenuItemsAsync();
            }

            // Обходим дерево меню для нахождения системного названия.
            result.SysName = SearchProfileMenuByText(redisData.ProfileMenuItems, text);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод обходит дерево меню профиля и находит системное имя пункта по его названию.
    /// Для обхода используем обычный проход, так как рекурсивный обход тут не нужен, дерево небольшое и его глубина заранее известна.
    /// </summary>
    /// <param name="profileMenuItems">Список меню.</param>
    /// <param name="text">Название.</param>
    /// <returns>Системное имя.</returns>
    private string SearchProfileMenuByText(IReadOnlyCollection<ProfileMenuItemsRedis> profileMenuItems, string text)
    {
        var stop = false;
        var model = CreateFactoryModelFromRedis(profileMenuItems);
        var items = model.ProfileMenuItems;
        var sysName = string.Empty;

        // Смотрим уровни дерева и находим нажатый пункт меню, далее берем его системное название и возвращаем фронту.
        foreach (var item in items)
        {
            // Пропускаем итерацию, если не нашли.
            if (!item.Label.Equals(text))
            {
                continue;
            }

            // Нашли, запишем и выходим с текущего уровня дерева выше.
            sysName = item.SysName;
            stop = true;
            break;
        }

        // На первом уровне нет нужного пункта, спускаемся глубже.
        if (!stop)
        {
            // Смотрим глубину дерева у первого уровня
            foreach (var firstLevelItem in items)
            {
                // Если уже нашли, выходим.
                if (stop)
                {
                    break;
                }

                // Смотрим глубину второго уровня.
                foreach (var secondLevelItem in firstLevelItem.Items)
                {
                    // Пропускаем итерацию, если не нашли.
                    if (!secondLevelItem.Label.Equals(text))
                    {
                        continue;
                    }

                    // Нашли, запишем и выходим с текущего уровня дерева выше.
                    sysName = secondLevelItem.SysName;
                    stop = true;
                    break;
                }
            }
        }

        return sysName;
    }

    /// <inheritdoc />
    public async Task<List<SkillOutput>> SelectedProfileUserSkillsAsync(Guid? userCode, string account)
    {
        try
        {
            var userId = await GetUserIdByUserCodeAsync(userCode, account);

            // Получаем навыки пользователя.
            var items = await _profileRepository.SelectedProfileUserSkillsAsync(userId!.Value);
            
            if (!items.Any())
            {
                return new List<SkillOutput>();
            }

            // Получаем всю информацию о навыках наполняя список.
            var skillsInfo = await _profileRepository.GetProfileSkillsBySkillIdAsync(items.Select(i => i.SkillId)
                .ToArray());
            
            var result = _mapper.Map<List<SkillOutput>>(skillsInfo);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<IntentOutput>> SelectedProfileUserIntentsAsync(Guid? userCode, string account)
    {
        try
        {
            var userId = await GetUserIdByUserCodeAsync(userCode, account);

            // Получаем навыки пользователя.
            var items = await _profileRepository.SelectedProfileUserIntentsAsync(userId!.Value);
            
            if (!items.Any())
            {
                return new List<IntentOutput>();
            }

            // Получаем всю информацию о навыках наполняя список.
            var skillsInfo = await _profileRepository.GetProfileIntentsByIntentIdAsync(items.Select(i => i.IntentId)
                .ToArray());
            var result = _mapper.Map<List<IntentOutput>>(skillsInfo);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает данные профиля.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <returns>Данные профиля.</returns>
    public async Task<ProfileInfoOutput> GetProfileInfoAsync(long profileInfoId)
    {
        try
        {
            if (profileInfoId <= 0)
            {
                var ex = new ArgumentNullException($"Не передан ProfileInfoId. ProfileInfoId: {profileInfoId}");
                throw ex;
            }

            var userId = await _userRepository.GetUserIdByProfileInfoIdAsync(profileInfoId);

            if (userId <= 0)
            {
                var ex = new InvalidOperationException($"Не найдено пользователя с ProfileInfoId: {profileInfoId}");
                throw ex;
            }

            var account = await _userRepository.GetUserAccountByUserIdAsync(userId);

            if (string.IsNullOrEmpty(account))
            {
                var ex = new InvalidOperationException("Не найдено аккаунта пользователя с " +
                                                       $"UserId: {userId}. " +
                                                       $"ProfileInfoId: {profileInfoId}");
                throw ex;
            }

            var result = await GetProfileInfoAsync(account);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
    
    /// <summary>
    /// Метод обновляет статус замечаниям на статус "На проверке", если есть неисправленные.
    /// </summary>
    /// <param name="profileInfoId">Id профиля.</param>
    private async Task CheckAwaitingCorrectionRemarksAsync(long profileInfoId)
    {
        var remarks = await _resumeModerationRepository.GetResumeRemarksAsync(profileInfoId);

        if (!remarks.Any())
        {
            return;
        }

        var awaitingRemarks = new List<ResumeRemarkEntity>();
        
        foreach (var r in remarks)
        {
            if (r.RemarkStatusId != (int)RemarkStatusEnum.AwaitingCorrection)
            {
                continue;
            }

            r.RemarkStatusId = (int)RemarkStatusEnum.Review;
            awaitingRemarks.Add(r);
        }

        if (awaitingRemarks.Any())
        {
            await _resumeModerationRepository.UpdateResumeRemarksAsync(awaitingRemarks);
        }
    }

    /// <summary>
    /// Метод получает Id пользователя по его коду либо по аккаунту.
    /// </summary>
    /// <param name="userCode">Код пользователя.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Id пользователя. Может вернуть null.</returns>
    /// <exception cref="InvalidOperationException">Если не удалось получить Id пользователя.</exception>
    private async Task<long?> GetUserIdByUserCodeAsync(Guid? userCode, string account)
    {
        long? userId;

        // Если просматриваем анкету другого пользователя.
        if (userCode is not null && userCode != Guid.Empty)
        {
            userId = await _userRepository.GetUserIdByCodeAsync(userCode.Value);   
        }

        // Если просматриваем свою анкету.
        else
        {
            userId = await _userRepository.GetUserByEmailAsync(account);   
        }

        if (userId is null || userId <= 0)
        {
            throw new InvalidOperationException("Не удалось получить Id пользователя. " +
                                                $"UserCode: {userCode}." +
                                                $"Account: {account}.");
        }

        return userId;
    }
}