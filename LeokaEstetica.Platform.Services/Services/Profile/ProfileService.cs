using AutoMapper;
using LeokaEstetica.Platform.Database.Abstractions.Profile;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Input.Profile;
using LeokaEstetica.Platform.Models.Dto.Output.Profile;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Redis.Abstractions;
using LeokaEstetica.Platform.Redis.Models;
using LeokaEstetica.Platform.Services.Abstractions.Profile;
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

    public ProfileService(ILogService logger,
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        IMapper mapper,
        IRedisService redisService)
    {
        _logger = logger;
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _redisService = redisService;
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
    /// Метод сохраняет данные контактной информации пользователя.
    /// </summary>
    /// <param name="profileInfoInput">Входная модель.</param>
    /// <param name="account">ккаунт пользователя.</param>
    /// <returns>Сохраненные данные.</returns>
    public async Task<ProfileInfoOutput> SaveProfileInfoAsync(ProfileInfoInput profileInfoInput, string account)
    {
        try
        {
            ValidateProfileInfo(profileInfoInput);

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
            var result = _mapper.Map<ProfileInfoOutput>(savedProfileInfo);
            
            // Сохраняем почту и номер телефона пользователя.
            await _userRepository.SaveUserPhoneAsync(userId, profileInfoInput.PhoneNumber);

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
    private void ValidateProfileInfo(ProfileInfoInput profileInfoInput)
    {
        if (string.IsNullOrEmpty(profileInfoInput.FirstName))
        {
            throw new ArgumentException("Имя должно быть заполнено!");
        }

        if (string.IsNullOrEmpty(profileInfoInput.LastName))
        {
            throw new ArgumentException("Фамилия должна быть заполнена!");
        }

        if (string.IsNullOrEmpty(profileInfoInput.Aboutme))
        {
            throw new ArgumentException("Информация о себе должна быть заполнена!");
        }

        // TODO: Добавить валидацию почты через регулярку. Завести для этого класс валидатора и туда эту валидацию.
        if (string.IsNullOrEmpty(profileInfoInput.Email))
        {
            throw new ArgumentException("Email пользователя должен быть заполнен!");
        }

        // TODO: Добавить валидацию номера телефона через регулярку. Завести для этого класс валидатора и туда эту валидацию.
        if (string.IsNullOrEmpty(profileInfoInput.PhoneNumber))
        {
            throw new ArgumentException("Номер телефона пользователя должнен быть заполнен!");
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
}