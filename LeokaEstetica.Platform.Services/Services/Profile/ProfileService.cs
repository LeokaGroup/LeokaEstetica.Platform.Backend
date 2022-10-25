using AutoMapper;
using LeokaEstetica.Platform.Database.Abstractions.Profile;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Input.Profile;
using LeokaEstetica.Platform.Models.Dto.Output.Profile;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Services.Abstractions.Profile;

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

    public ProfileService(ILogService logger, 
        IProfileRepository profileRepository, 
        IUserRepository userRepository, 
        IMapper mapper)
    {
        _logger = logger;
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _mapper = mapper;
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
            
            var profileInfo = await _profileRepository.GetProfileInfoAsync(userId);

            if (profileInfo is null)
            {
                throw new NullReferenceException($"Для пользователя {account} не заведено профиля в системе!");
            }

            CreateProfileInfoModel(profileInfoInput, userId, ref profileInfo);
            var savedProfileInfo = await _profileRepository.SaveProfileInfoAsync(profileInfo);
            var result = _mapper.Map<ProfileInfoOutput>(savedProfileInfo);

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
    }

    /// <summary>
    /// Метод создает модель для сохранения данных профиля пользователя.
    /// </summary>
    /// <param name="profileInfoInput">Входная модель.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="profileInfoId">Id профиля пользователя.</param>
    private void CreateProfileInfoModel(ProfileInfoInput profileInfoInput, long userId, ref ProfileInfoEntity profileInfo)
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
}