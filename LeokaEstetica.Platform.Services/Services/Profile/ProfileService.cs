using AutoMapper;
using LeokaEstetica.Platform.Database.Abstractions.Profile;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Profile;
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
}