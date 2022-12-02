using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using LeokaEstetica.Platform.Access.Helpers;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Profile;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Entities.User;
using LeokaEstetica.Platform.Services.Abstractions.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

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
    private readonly PgContext _pgContext;
    private readonly IProfileRepository _profileRepository;
    
    public UserService(ILogService logger, 
        IUserRepository userRepository, 
        IMapper mapper, 
        IMailingsService mailingsService, 
        PgContext pgContext, 
        IProfileRepository profileRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _mapper = mapper;
        _mailingsService = mailingsService;
        _pgContext = pgContext;
        _profileRepository = profileRepository;
    }

    /// <summary>
    /// Метод создает нового пользователя.
    /// </summary>
    /// <param name="password">Пароль. Он не хранится в БД. Хранится только его хэш.</param>
    /// <param name="email">Почта пользователя.</param>
    /// <returns>Данные пользователя.</returns>
    public async Task<UserSignUpOutput> CreateUserAsync(string password, string email)
    {
        var tran = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);
        
        try
        {
            var result = new UserSignUpOutput();
            await CheckUserByEmailAsync(result, email);

            var userModel = CreateSignUpUserModel(password, email);
            
            var userId = await _userRepository.SaveUserAsync(userModel);
            ValidateUserId(result, userId);

            if (result.Errors.Any())
            {
                return result;
            }

            // Находим добавленного пользователя.
            var addedUser = await _userRepository.GetUserByUserIdAsync(userId);
            
            if (addedUser is null)
            {
                throw new NullReferenceException("Ошибка добавления пользователя!");
            }
            
            result = _mapper.Map<UserSignUpOutput>(addedUser);
            
            // Добавляет данные о пользователе в таблицу профиля.
            await _profileRepository.AddUserInfoAsync(userId);

            var confirmationEmailCode = Guid.NewGuid();
            
            // Записываем пользавателю код подтверждения для проверки его позже из его почты по ссылке.
            await _userRepository.SetConfirmAccountCodeAsync(confirmationEmailCode, addedUser.UserId);
            
            // Отправляем пользователю письмо подтверждения почты.
            await _mailingsService.SendConfirmEmailAsync(email, confirmationEmailCode);
            
            await tran.CommitAsync();

            return result;
        }

        catch (NullReferenceException ex)
        {
            await tran.RollbackAsync();
            await _logger.LogCriticalAsync(ex);
            throw;
        }

        catch (Exception ex)
        {
            await tran.RollbackAsync();
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
            result.Errors = new List<ValidationFailure>()
            {
                new() { ErrorMessage = $"Пользователь с Email {email} уже зарегистрирован в системе!" }
            };
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
            result.Errors = new List<ValidationFailure>()
            {
                new() { ErrorMessage = "Id пользователя был <= 0!" }
            };
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

    /// <summary>
    /// Метод авторизует пользователя.
    /// </summary>
    /// <param name="email">Email.</param>
    /// <param name="password">Пароль.</param>
    /// <returns>Данные авторизации.</returns>
    public async Task<UserSignInOutput> SignInAsync(string email, string password)
    {
        try
        {
            var result = new UserSignInOutput();

            var passwordHash = await _userRepository.GetPasswordHashByEmailAsync(email);

            if (passwordHash is null)
            {
                throw new NullReferenceException($"Хэш пароль не удалось получить для пользователя {email}");
            }

            var checkPassword = HashHelper.VerifyHashedPassword(passwordHash, password);

            if (!checkPassword)
            {
                throw new UnauthorizedAccessException("Пользователь не прошел проверку по паролю!");
            }

            var userCode = await _userRepository.GetUserCodeByEmailAsync(email);

            var claim = GetIdentityClaim(email);
            var token = CreateTokenFactory(claim);
            
            result.Email = email;
            result.Token = token;
            result.IsSuccess = true;
            result.UserCode = userCode;

            return result;
        }
        
        catch (Exception ex)
        {
            await _logger.LogCriticalAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод выдает токен пользователю, если он прошел авторизацию.
    /// </summary>
    /// <param name="email">Email.</param>
    /// <returns>Токен пользователя.</returns>
    private ClaimsIdentity GetIdentityClaim(string email)
    {
        var claims = new List<Claim> {
            new(ClaimsIdentity.DefaultNameClaimType, email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Token", 
            ClaimsIdentity.DefaultNameClaimType, 
            ClaimsIdentity.DefaultRoleClaimType);

        return claimsIdentity;
    }
    
    /// <summary>
    /// Метод создает токен пользователю.
    /// </summary>
    /// <param name="claimsIdentity">Объект полномочий.</param>
    /// <returns>Строка токена.</returns>
    private string CreateTokenFactory(ClaimsIdentity claimsIdentity)
    {
        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: claimsIdentity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return encodedJwt;
    }
    
    /// <summary>
    /// Метод обновляет токен.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Новые данные авторизации.</returns>
    public async Task<UserSignInOutput> RefreshTokenAsync(string account)
    {
        try
        {
            var claim = GetIdentityClaim(account);
            var token = CreateTokenFactory(claim);

            var result = new UserSignInOutput
            {
                Token = token
            };

            return result;
        }
        
        catch (Exception ex)
        {
            await _logger.LogCriticalAsync(ex);
            throw;
        }
    }
}