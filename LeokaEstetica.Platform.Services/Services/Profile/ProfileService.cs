using AutoMapper;
using LeokaEstetica.Platform.Database.Abstractions.Profile;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Base.Profile;
using LeokaEstetica.Platform.Models.Dto.Input.Profile;
using LeokaEstetica.Platform.Models.Dto.Output.Profile;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Redis.Abstractions;
using LeokaEstetica.Platform.Redis.Models;
using LeokaEstetica.Platform.Services.Abstractions.Profile;
using LeokaEstetica.Platform.Services.Consts;
using LeokaEstetica.Platform.Services.Validators;
using Items = LeokaEstetica.Platform.Redis.Models.Items;

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
    private readonly IRedisService _redisService;
    private readonly INotificationsService _notificationsService;

    public ProfileService(ILogService logger,
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        IMapper mapper,
        IRedisService redisService, 
        INotificationsService notificationsService)
    {
        _logger = logger;
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _redisService = redisService;
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
                throw new ArgumentException("Имя аккаунта не передано!");
            }

            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                throw new NullReferenceException($"Пользователя с почтой {account} не существует в системе!");
            }

            var profileInfo = await _profileRepository.GetProfileInfoAsync(userId);

            if (profileInfo is null)
            {
                throw new NullReferenceException($"Не найдено информации о профиле пользователя с UserId: {userId}");
            }

            var result = _mapper.Map<ProfileInfoOutput>(profileInfo);

            // Получаем поля почты и номера телефона пользователя.
            var userData = await _userRepository.GetUserPhoneEmailByUserIdAsync(userId);

            if (userData is null)
            {
                return result;
            }

            result.Email = userData.Email;
            result.PhoneNumber = userData.PhoneNumber;

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
            await _redisService.SaveProfileMenuCacheAsync(model);

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
    /// <returns>Список навыков.</returns>
    public async Task<IEnumerable<SkillOutput>> ProfileSkillsAsync()
    {
        try
        {
            var items = await _profileRepository.ProfileSkillsAsync();
            var result = _mapper.Map<IEnumerable<SkillOutput>>(items);

            return result;
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
    /// <returns>Список целей.</returns>
    public async Task<IEnumerable<IntentOutput>> ProfileIntentsAsync()
    {
        try
        {
            var items = await _profileRepository.ProfileIntentsAsync();
            var result = _mapper.Map<IEnumerable<IntentOutput>>(items);

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
            var result = new ProfileInfoOutput();
            ValidateProfileInfo(profileInfoInput, ref result);

            // Если есть ошибки валидации, не разрешаем идти дальше. Выдаем ошибки фронту.
            if (result.Errors.Any())
            {
                result.IsSuccess = false;
                return result;
            }

            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new NullReferenceException($"Id пользователя с аккаунтом {account} не найден!");
            }

            // Получаем данные профиля пользователя.
            var profileInfo = await _profileRepository.GetProfileInfoAsync(userId);

            if (profileInfo is null)
            {
                throw new NullReferenceException($"Для пользователя {account} не заведено профиля в системе!");
            }

            CreateProfileInfoModel(profileInfoInput, ref profileInfo);

            // Сохраняем данные пользователя.
            var savedProfileInfo = await _profileRepository.SaveProfileInfoAsync(profileInfo);
            result = _mapper.Map<ProfileInfoOutput>(savedProfileInfo);

            // Сохраняем номер телефона пользователя.
            await _userRepository.SaveUserPhoneAsync(userId, profileInfoInput.PhoneNumber);
            
            // Сохраняем выбранные навыки пользователя.
            await _profileRepository.SaveProfileSkillsAsync(profileInfoInput.UserSkills.Select(s => new UserSkillEntity
            {
                SkillId = s.SkillId,
                UserId = userId,
                Position = s.Position
            }));

            // Отправляем уведомление о сохранении фронту.
            await _notificationsService.SendNotifySuccessSaveAsync("Все хорошо", "Данные успешно сохранены!", null);

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
    /// Метод валидирует входную модель контактной информации.
    /// </summary>
    /// <param name="profileInfoInput">Входная модель для валидации.</param>
    private void ValidateProfileInfo(ProfileInfoInput profileInfoInput, ref ProfileInfoOutput result)
    {
        // Проверка фамилии.
        if (string.IsNullOrEmpty(profileInfoInput.FirstName))
        {
            var error = new ArgumentException(ValidationConsts.EMPTY_FIRST_NAME_ERROR);
            _logger.LogError(error);
            result.Errors.Add(ValidationConsts.EMPTY_FIRST_NAME_ERROR);
        }

        // Проверка имени.
        if (string.IsNullOrEmpty(profileInfoInput.LastName))
        {
            var error = new ArgumentException(ValidationConsts.EMPTY_LAST_NAME_ERROR);
            _logger.LogError(error);
            result.Errors.Add(ValidationConsts.EMPTY_LAST_NAME_ERROR);
        }

        // Проверка информации о себе.
        if (string.IsNullOrEmpty(profileInfoInput.Aboutme))
        {
            var error = new ArgumentException(ValidationConsts.EMPTY_ABOUTME_ERROR);
            _logger.LogError(error);
            result.Errors.Add(ValidationConsts.EMPTY_ABOUTME_ERROR);
        }
        
        // Проверка почты.
        if (string.IsNullOrEmpty(profileInfoInput.Email))
        {
            var error = new ArgumentException(ValidationConsts.EMPTY_EMAIL_ERROR);
            _logger.LogError(error);
            result.Errors.Add(ValidationConsts.EMPTY_EMAIL_ERROR);
            profileInfoInput.Email = string.Empty;
        }

        // Проверка формата почты.
        if (!UserValidator.IsValidEmail(profileInfoInput.Email))
        {
            var error = new ArgumentException(ValidationConsts.NOT_VALID_EMAIL_ERROR);
            _logger.LogError(error);
            result.Errors.Add(ValidationConsts.NOT_VALID_EMAIL_ERROR);
        }
        
        // Проверка номера телефона.
        if (string.IsNullOrEmpty(profileInfoInput.PhoneNumber))
        {
            var error = new ArgumentException(ValidationConsts.EMPTY_PHONE_NUMBER_ERROR);
            _logger.LogError(error);
            result.Errors.Add(ValidationConsts.EMPTY_PHONE_NUMBER_ERROR);
            profileInfoInput.PhoneNumber = string.Empty;
        }

        // Проверка формата номера телефона.
        if (!UserValidator.IsValidPhoneNumber(profileInfoInput.PhoneNumber))
        {
            var error = new ArgumentException(ValidationConsts.NOT_VALID_PHONE_NUMBER_ERROR);
            _logger.LogError(error);
            result.Errors.Add(ValidationConsts.NOT_VALID_PHONE_NUMBER_ERROR);
        }
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
                Items = pmi.Items.Select(i => new Items
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
                Items = pmi.Items.Select(i => new Items
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
            var redisData = await _redisService.GetProfileMenuCacheAsync();

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
    /// Метод сохраняет выбранные пользователем навыки.
    /// </summary>
    /// <param name="selectedSkills">Список навыков для сохранения.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список навыков.</returns>
    public async Task SaveProfileSkillsAsync(IEnumerable<SkillInput> selectedSkills, string account)
    {
        try
        {
            var result = new SaveUserSkillOutput();
            var skillInputs = selectedSkills.ToList();
            
            if (!skillInputs.Any())
            {
                result.Errors.Add("Не передан список навыков для сохранения!");
            }

            var userId = await _userRepository.GetUserByEmailAsync(account);
            
            // Получаем список навыков из БД, чтобы проставить флаги тем, которые выбрал пользователь.
            var allSkills = await ProfileSkillsAsync();
            var skillOutputs = allSkills.ToList();
            
            if (!skillOutputs.Any())
            {
                throw new NullReferenceException("Не удалось получить список навыков для сохранения!");
            }

            // Получаем пересечения между элементами, которым будем проставлять флаг.
            var items = skillOutputs
                .Intersect<BaseSkill>(skillInputs)
                .ToList();

            // Если нет пересечений между элементами.
            if (!items.Any())
            {
                throw new NullReferenceException("Нет пересечений между элементами навыков для сохранения!");
            }
            
            // Проставляем флаг выбранным элементам.
            items.ForEach(i => i.IsSelected = true);
            
            // Сохраняем навыки пользователя в базу.
            await _profileRepository.SaveProfileSkillsAsync(items.Select(i => new UserSkillEntity
            {
                UserId = userId,
                SkillId = i.SkillId,
                Position = i.Position
            }));
        }
        
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает выбранные пользователям навыки.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список навыков.</returns>
    public async Task<IEnumerable<SkillOutput>> SelectedProfileUserSkillsAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);
        
            // Получаем навыки пользователя.
            var items = await _profileRepository.SelectedProfileUserSkillsAsync(userId);

            var userSkillEntities = items.ToList();
            if (!userSkillEntities.Any())
            {
                return Enumerable.Empty<SkillOutput>();
            }
        
            // Получаем всю информацию о навыках наполняя список.
            var skillsInfo = await _profileRepository.GetProfileSkillsBySkillIdAsync(userSkillEntities.Select(i => i.SkillId).ToArray());
            var result = _mapper.Map<IEnumerable<SkillOutput>>(skillsInfo);

            return result;
        }
        
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex);
            throw;
        }
    }
}