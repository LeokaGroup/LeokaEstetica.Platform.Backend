using AutoMapper;
using LeokaEstetica.Platform.Database.Abstractions.Profile;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Input.Profile;
using LeokaEstetica.Platform.Models.Dto.Output.Profile;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Redis.Abstractions.Profile;
using LeokaEstetica.Platform.Redis.Models.Profile;
using LeokaEstetica.Platform.Services.Abstractions.Profile;
using Items = LeokaEstetica.Platform.Models.Dto.Output.Profile.ProfileItems;
using ProfileItems = LeokaEstetica.Platform.Redis.Models.Profile.ProfileItems;

namespace LeokaEstetica.Platform.Services.Services.Profile;

/// <summary>
/// Класс реализует методы сервиса профиля пользователя.
/// </summary>
public sealed class ProfileService : IProfileService
{
    private readonly ILogService _logger;
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IProfileRedisService _profileRedisService;
    private readonly INotificationsService _notificationsService;

    public ProfileService(ILogService logger,
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        IMapper mapper,
        IProfileRedisService profileRedisService,
        INotificationsService notificationsService)
    {
        _logger = logger;
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _profileRedisService = profileRedisService;
        _notificationsService = notificationsService;
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

            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                throw new InvalidOperationException($"Пользователя с почтой {account} не существует в системе.");
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
            //Заполняем строку WarningsCommments если есть незаполеннные поля.
            result.WarningComment = SetWarningCommentAsync(result, account).Result;

            return result;
        }

        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex);
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
            await _logger.LogErrorAsync(ex);
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
            var userSkills = await SelectedProfileUserSkillsAsync(account);

            // Находим Id навыков, которые ранее были выбраны пользователем.
            var ids = userSkills.Select(s => s.SkillId);
            result.RemoveAll(s => ids.Contains(s.SkillId));

            return result;;
        }

        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex);
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
            var userIntents = await SelectedProfileUserIntentsAsync(account);

            // Находим Id целей, которые ранее были выбраны пользователем.
            var ids = userIntents.Select(i => i.IntentId);
            result.RemoveAll(s => ids.Contains(s.IntentId));

            return result;
        }

        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод сохраняет данные анкеты пользователя.
    /// </summary>
    /// <param name="profileInfoInput">Входная модель.</param>
    /// <param name="account">ккаунт пользователя.</param>
    /// <returns>Сохраненные данные.</returns>
    public async Task<ProfileInfoOutput> SaveProfileInfoAsync(ProfileInfoInput profileInfoInput, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            // Получаем данные профиля пользователя.
            var profileInfo = await _profileRepository.GetProfileInfoAsync(userId);

            if (profileInfo is null)
            {
                throw new InvalidOperationException($"Для пользователя {account} не заведено профиля в системе.");
            }

            CreateProfileInfoModel(profileInfoInput, ref profileInfo);

            // Сохраняем данные пользователя.
            var savedProfileInfo = await _profileRepository.SaveProfileInfoAsync(profileInfo);
            var result = _mapper.Map<ProfileInfoOutput>(savedProfileInfo);
            result.PhoneNumber = profileInfoInput.PhoneNumber;

            // Сохраняем номер телефона пользователя.
            await _userRepository.SaveUserPhoneAsync(userId, profileInfoInput.PhoneNumber);

            // Сохраняем выбранные навыки пользователя.
            await SaveUserSkillsAsync(profileInfoInput, userId);
            
            // Сохраняем выбранные цели пользователя.
            await SaveUserIntentsAsync(profileInfoInput, userId);

            // Отправляем уведомление о сохранении фронту.
            await _notificationsService.SendNotifySuccessSaveAsync("Все хорошо", "Данные успешно сохранены.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS);

            result.IsSuccess = true;

            return result;
        }

        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex);
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
                    .ToList()
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
            await _logger.LogErrorAsync(ex);
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

    /// <summary>
    /// Метод получает выбранные пользователям навыки.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список навыков.</returns>
    public async Task<List<SkillOutput>> SelectedProfileUserSkillsAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            // Получаем навыки пользователя.
            var items = await _profileRepository.SelectedProfileUserSkillsAsync(userId);
            
            if (!items.Any())
            {
                return new List<SkillOutput>();
            }

            // Получаем всю информацию о навыках наполняя список.
            var skillsInfo = await _profileRepository.GetProfileSkillsBySkillIdAsync(items.Select(i => i.SkillId).ToArray());
            var result = _mapper.Map<List<SkillOutput>>(skillsInfo);

            return result;
        }

        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает выбранные пользователем цели.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список целей.</returns>
    public async Task<List<IntentOutput>> SelectedProfileUserIntentsAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            // Получаем навыки пользователя.
            var items = await _profileRepository.SelectedProfileUserIntentsAsync(userId);
            
            if (!items.Any())
            {
                return new List<IntentOutput>();
            }

            // Получаем всю информацию о навыках наполняя список.
            var skillsInfo = await _profileRepository.GetProfileIntentsByIntentIdAsync(items.Select(i => i.IntentId).ToArray());
            var result = _mapper.Map<List<IntentOutput>>(skillsInfo);

            return result;
        }

        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод возвращает строку warnings если некоторые поля у выходной модели не заполнены.
    /// </summary>
    /// <param name="profilefoOutput">Возвращаемая модель профиля.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Строка предупреждения.</returns>
    private async Task<string> SetWarningCommentAsync(ProfileInfoOutput profilefoOutput, string account)
    {
        // Получаем скиллы и целт пользователя
        var skills = SelectedProfileUserSkillsAsync(account).Result.ToList();
        var intents = SelectedProfileUserIntentsAsync(account).Result.ToList();

        string warnings = "Ваша анкета не попадет в базу резюме, пока не будут заполнены поля: ";

        if (string.IsNullOrEmpty(profilefoOutput.FirstName))
        {
            warnings += "имя, ";
        }
        if (string.IsNullOrEmpty(profilefoOutput.LastName))
        {
            warnings += "фамилия, ";
        }
        if (string.IsNullOrEmpty(profilefoOutput.Email))
        {
            warnings += "email, ";
        }
        if (string.IsNullOrEmpty(profilefoOutput.PhoneNumber))
        {
            warnings += "номер телефона, ";
        }
        if (string.IsNullOrEmpty(profilefoOutput.Aboutme))
        {
            warnings += "опыт (стаж), ";
        }
        if (string.IsNullOrEmpty(profilefoOutput.Job))
        {
            warnings += "должность, ";
        }
        if (skills.Count() < 1)
        {
            warnings += "ваши навыки, ";
        }
        if (intents.Count() < 1)
        {
            warnings += "ваши цели на платформе, ";
        }
        warnings = warnings.Remove(warnings.Count() - 2);
        warnings += ".";

        return warnings;
    }
}