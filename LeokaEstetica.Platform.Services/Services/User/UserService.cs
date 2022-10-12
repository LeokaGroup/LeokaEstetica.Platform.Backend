using AutoMapper;
using LeokaEstetica.Platform.Access.Helpers;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
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
    private readonly IMailingsService _mailingsService;
    
    public UserService(ILogService logger, 
        IUserRepository userRepository, 
        IMapper mapper, 
        IMailingsService mailingsService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _mapper = mapper;
        _mailingsService = mailingsService;
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
            var result = new UserSignUpOutput();
            ValidateSignUpParams(result, password, email);
            await CheckUserByEmailAsync(result, email);

            var userModel = CreateSignUpUserModel(password, email);
            var userId = await _userRepository.SaveUserAsync(userModel);
            ValidateUserId(result, userId);

            if (result.Errors.Any())
            {
                return result;
            }

            // Находим добавленного пользователя.
            // var addedUser = await _userRepository.GetUserByUserIdAsync(userId);
            //
            // if (addedUser is null)
            // {
            //     throw new NullReferenceException("Ошибка добавления пользователя!");
            // }
            //
            // result = _mapper.Map<UserSignUpOutput>(addedUser);
                
            var confirmationEmailCode = Guid.NewGuid();
            
            // Записываем пользлвателю код подтверждения для проверки его позже из его почты по ссылке.
            // await _userRepository.SetConfirmAccountCodeAsync(confirmationEmailCode, addedUser.UserId);
            
            // Отправляем пользователю письмо подтверждения почты.
            await _mailingsService.SendConfirmEmailAsync(email, confirmationEmailCode);

            return result;
        }

        catch (NullReferenceException ex)
        {
            await _logger.LogCriticalAsync(ex);
            throw;
        }

        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод проверяет существование пользователя в базе по email.
    /// </summary>
    /// <param name="email">Email пользователя.</param>
    private async Task CheckUserByEmailAsync(UserSignUpOutput result, string email)
    {
        var isUser = await _userRepository.CheckUserByEmailAsync(email);
        
        // Пользователь уже есть, не даем регистрировать.
        if (isUser)
        {
            result.Errors = new List<string> { $"Пользователь с Email {email} уже зарегистрирован в системе!" };
        }
    }

    /// <summary>
    /// Метод проверяет входные параметры. Генерит исключения, если что то не так.
    /// </summary>
    /// <param name="password">Пароль./param>
    /// <param name="email">Почта.</param>
    private void ValidateSignUpParams(UserSignUpOutput result, string password, string email)
    {
        if (string.IsNullOrEmpty(password))
        {
            result.Errors = new List<string> { "Пароль не может быть пустым!" };
        }

        if (string.IsNullOrEmpty(email))
        {
            result.Errors = new List<string> { "Email не может быть пустым!" };
        }
    }

    /// <summary>
    /// Метод создает модель для регистрации пользователя.
    /// </summary>
    /// <param name="password">Пароль./param>
    /// <param name="email">Почта.</param>
    /// <returns>Модель с данными.</returns>
    private UserEntity CreateSignUpUserModel(string password, string email)
    {
        var model = new UserEntity
        {
            PasswordHash = HashHelper.HashPassword(password),
            Email = email,
            DateRegister = DateTime.UtcNow,
            UserCode = Guid.NewGuid()
        };

        return model;
    }

    /// <summary>
    /// Метод проверяет UserId. Сроздает исключение, если с ним проблемы.
    /// </summary>
    /// <param name="userId">UserId.</param>
    private void ValidateUserId(UserSignUpOutput result, long userId)
    {
        try
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Id пользователя был <= 0!");
            }
        }
        
        catch (ArgumentException ex)
        {
            result.Errors = new List<string> { "Id пользователя был <= 0!" };
            _logger.LogCritical(ex);
        }
    }
    
    /// <summary>
    /// Метод подтверждает аккаунт пользователя по коду, который ранее был отправлен пользователю на почту и записан в БД.
    /// </summary>
    /// <param name="code">Код подтверждения.</param>
    /// <returns>Статус подтверждения.</returns>
    public async Task<bool> ConfirmAccountAsync(Guid code)
    {
        try
        {
            var result = await _userRepository.ConfirmAccountAsync(code);

            return result;
        }
        
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex);
            throw;
        }
    }
}