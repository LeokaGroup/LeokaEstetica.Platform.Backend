using AutoMapper;
using LeokaEstetica.Platform.Access.Helpers;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Entities.User;
using LeokaEstetica.Platform.Services.Abstractions.User;

namespace LeokaEstetica.Platform.Services.Services.User;

/// <summary>
/// Класс реализует методы сервиса пользователей.
/// </summary>
public sealed class UserService : IUserService
{
    private readonly ILogService _logger;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    
    public UserService(ILogService logger, 
        IUserRepository userRepository, 
        IMapper mapper)
    {
        _logger = logger;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод создает нового пользователя.
    /// </summary>
    /// <param name="password">Пароль. Он не хранится в БД. Хранится только его хэш.</param>
    /// <param name="email">Почта пользователя.</param>
    /// <returns>Данные пользователя.</returns>
    public async Task<UserSignUpOutput> CreateUserAsync(string password, string email)
    {
        try
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new NullReferenceException("Пароль не может быть пустым!");
            }
            
            if (string.IsNullOrEmpty(email))
            {
                throw new NullReferenceException("Email не может быть пустым!");
            }

            var user = new UserEntity
            {
                PasswordHash = HashHelper.HashPassword(password),
                Email = email,
                DateRegister = DateTime.UtcNow
            };

            var newUser = await _userRepository.SaveUserAsync(user);
            var result = new UserSignUpOutput();

            if (newUser is not null)
            {
                result = _mapper.Map<UserSignUpOutput>(newUser);
            }

            return result;
        }
        
        catch (Exception ex)
        {
            await _logger.LogCriticalAsync(ex);
            throw;
        }
    }
}