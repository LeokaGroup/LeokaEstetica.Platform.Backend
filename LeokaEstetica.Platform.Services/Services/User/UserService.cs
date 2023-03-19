using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Access.Helpers;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Resume;
using LeokaEstetica.Platform.Database.Abstractions.Profile;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Models.Dto.Input.User;
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
public class UserService : IUserService
{
    private readonly ILogService _logger;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IMailingsService _mailingsService;
    private readonly PgContext _pgContext;
    private readonly IProfileRepository _profileRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IResumeModerationRepository _resumeModerationRepository;
    private readonly IAccessUserService _accessUserService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Сервис логера.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="mailingsService">Сервис рассылок.</param>
    /// <param name="pgContext">Датаконтекст.</param>
    /// <param name="profileRepository">Репозиторий профиля.</param>
    /// <param name="profileRepository">Репозиторий подписок.</param>
    /// <param name="resumeModerationRepository">Репозиторий модерации анкет.</param>
    public UserService(ILogService logger, 
        IUserRepository userRepository, 
        IMapper mapper, 
        IMailingsService mailingsService, 
        PgContext pgContext, 
        IProfileRepository profileRepository, 
        ISubscriptionRepository subscriptionRepository, 
        IResumeModerationRepository resumeModerationRepository, 
        IAccessUserService accessUserService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _mapper = mapper;
        _mailingsService = mailingsService;
        _pgContext = pgContext;
        _profileRepository = profileRepository;
        _subscriptionRepository = subscriptionRepository;
        _resumeModerationRepository = resumeModerationRepository;
        _accessUserService = accessUserService;
    }

    #region Публичные методы.

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
            var result = new UserSignUpOutput { Errors = new List<ValidationFailure>() };
            await CheckUserByEmailAsync(result, email);

            var userModel = CreateSignUpUserModel(password, email);
            
            var userId = await _userRepository.AddUserAsync(userModel);
            ValidateUserId(result, userId);

            if (result.Errors.Any())
            {
                return result;
            }

            // Находим добавленного пользователя.
            var addedUser = await _userRepository.GetUserByUserIdAsync(userId);
            
            if (addedUser is null)
            {
                throw new InvalidOperationException("Ошибка добавления пользователя.");
            }
            
            result = _mapper.Map<UserSignUpOutput>(addedUser);
            
            // Добавляет данные о пользователе в таблицу профиля.
            var profileInfoId = await _profileRepository.AddUserInfoAsync(userId);

            var confirmationEmailCode = Guid.NewGuid();
            
            // Записываем пользавателю код подтверждения для проверки его позже из его почты по ссылке.
            await _userRepository.SetConfirmAccountCodeAsync(confirmationEmailCode, addedUser.UserId);
            
            // Отправляем пользователю письмо подтверждения почты.
            await _mailingsService.SendConfirmEmailAsync(email, confirmationEmailCode);
            
            // Добавляем пользователю бесплатную подписку.
            await _subscriptionRepository.AddUserSubscriptionAsync(addedUser.UserId, SubscriptionTypeEnum.FareRule, 1);
            
            // Отправляем анкету на модерацию.
            await _resumeModerationRepository.AddResumeModerationAsync(profileInfoId);

            await tran.CommitAsync();

            return result;
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
            DateRegister = DateTime.Now,
            UserCode = Guid.NewGuid()
        };

        return model;
    }
    
    /// <summary>
    /// Метод создает модель для регистрации пользователя.
    /// </summary>
    /// <param name="vkUserId">Id пользователя в системе ВК.</param>
    /// <param name="firstName">Имя пользователя в системе ВК.</param>
    /// <param name="firstName">Фамилия пользователя в системе ВК.</param>
    /// <returns>Модель с данными.</returns>
    private UserEntity CreateSignUpVkUserModel(long vkUserId, string firstName, string lastName)
    {
        var model = new UserEntity
        {
            PasswordHash = string.Empty,
            Email = string.Empty,
            DateRegister = DateTime.Now,
            UserCode = Guid.NewGuid(),
            VkUserId = vkUserId,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true,
            IsVkAuth = true
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
            var result = new UserSignInOutput { Errors = new List<ValidationFailure>() };

            var passwordHash = await _userRepository.GetPasswordHashByEmailAsync(email);

            if (passwordHash is null)
            {
                throw new InvalidOperationException($"Хэш пароль не удалось получить для пользователя {email}");
            }

            var checkPassword = HashHelper.VerifyHashedPassword(passwordHash, password);

            if (!checkPassword)
            {
                throw new UnauthorizedAccessException("Пользователь не прошел проверку по паролю.");
            }

            try
            {
                // Проверяем пользователя на блокировку.
                await CheckUserBlackListAsync(email, false);
            }
            
            catch (InvalidOperationException ex)
            {
                result.Errors.Add(new ValidationFailure
                {
                    ErrorMessage = ex.Message,
                    CustomState = "warn" // Чтобы на фронте отобразить как Warning.
                });

                return result;
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
    
    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод выдает токен пользователю, если он прошел авторизацию.
    /// </summary>
    /// <param name="email">Email.</param>
    /// <returns>Токен пользователя.</returns>
    private ClaimsIdentity GetIdentityClaim(string email)
    {
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Token", 
            ClaimsIdentity.DefaultNameClaimType, 
            ClaimsIdentity.DefaultRoleClaimType);

        return claimsIdentity;
    }
    
    /// <summary>
    /// Метод выдает токен пользователю, если он прошел авторизацию.
    /// </summary>
    /// <param name="vkUserId">VkUserId пользователя.</param>
    /// <returns>Токен пользователя.</returns>
    private ClaimsIdentity GetIdentityClaimVkUser(long vkUserId)
    {
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, vkUserId.ToString())
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
        var now = DateTime.Now;
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

    /// <summary>
    /// Метод авторизации через Google. Если аккаунт не зарегистрирован в системе,
    /// то создаем также аккаунт используя данные аккаунта Google пользователя.
    /// </summary>
    /// <param name="googleAuthToken">Токен с данными пользователя.</param>
    /// <returns>Данные пользователя.</returns>
    public async Task<UserSignInOutput> SignInAsync(string googleAuthToken)
    {
        var tran = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);
        
        try
        {
            var claims = DecodeJwtGoogleData(googleAuthToken);
            var googleUserData = CreateGoogleDataFactory(claims);
            
            // Првоеряем, существует ли такой пользователь в системе.
            var isUserExists = await _userRepository.CheckUserByEmailAsync(googleUserData.Email);
            
            // Пользователя нет, регистрируем его и выдаем доступ.
            if (!isUserExists)
            {
                var userModel = CreateSignUpUserModel(string.Empty, googleUserData.Email);
                userModel.EmailConfirmed = true; // Почта и так подтверждена, раз это Google аккаунт.
            
                var userId = await _userRepository.AddUserAsync(userModel);

                if (userId <= 0)
                {
                    throw new InvalidOperationException("Ошибка добавления пользователя.");
                }

                // Добавляет данные о пользователе в таблицу профиля.
                var profileInfoId = await _profileRepository.AddUserInfoAsync(userId);

                var confirmationEmailCode = Guid.NewGuid();
            
                // Записываем пользавателю код подтверждения для проверки его позже из его почты по ссылке.
                await _userRepository.SetConfirmAccountCodeAsync(confirmationEmailCode, userId);

                // Добавляем пользователю бесплатную подписку.
                await _subscriptionRepository.AddUserSubscriptionAsync(userId, SubscriptionTypeEnum.FareRule, 1);
            
                // Отправляем анкету на модерацию.
                await _resumeModerationRepository.AddResumeModerationAsync(profileInfoId);
            }

            // Если пользователь уже существует, то выдаем ему доступ.
            var userCode = await _userRepository.GetUserCodeByEmailAsync(googleUserData.Email);
            var claim = GetIdentityClaim(googleUserData.Email);
            var token = CreateTokenFactory(claim);

            var result = new UserSignInOutput
            {
                Errors = new List<ValidationFailure>(),
                Email = googleUserData.Email,
                Token = token,
                IsSuccess = true,
                UserCode = userCode
            };

            await tran.CommitAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод авторизации через VK. Если аккаунт не зарегистрирован в системе,
    /// то создаем также аккаунт используя данные аккаунта DR пользователя.
    /// </summary>
    /// <param name="vkUserId">Id пользователя в системе ВК.</param>
    /// <param name="firstName">Имя пользователя в системе ВК.</param>
    /// <param name="firstName">Фамилия пользователя в системе ВК.</param>
    /// <returns>Данные пользователя.</returns>
    public async Task<UserSignInOutput> SignInAsync(long vkUserId, string firstName, string lastName)
    {
        var result = new UserSignInOutput { Errors = new List<ValidationFailure>() };
        
        var tran = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);
        
        try
        {
            // Првоеряем, существует ли такой пользователь в системе.
            var isUserExists = await _userRepository.CheckUserByVkUserIdAsync(vkUserId);
            
            // Пользователя нет, регистрируем его и выдаем доступ.
            if (!isUserExists)
            {
                try
                {
                    // Проверяем пользователя на блокировку.
                    await CheckUserBlackListAsync(vkUserId.ToString(), true);
                }
            
                catch (InvalidOperationException ex)
                {
                    result.Errors.Add(new ValidationFailure
                    {
                        ErrorMessage = ex.Message,
                        CustomState = "warn" // Чтобы на фронте отобразить как Warning.
                    });

                    return result;
                }
                
                var userModel = CreateSignUpVkUserModel(vkUserId, firstName, lastName);

                var userId = await _userRepository.AddUserAsync(userModel);

                if (userId <= 0)
                {
                    throw new InvalidOperationException("Ошибка добавления пользователя.");
                }

                // Добавляет данные о пользователе в таблицу профиля.
                var profileInfoId = await _profileRepository.AddUserInfoAsync(userId);

                // Записываем пользавателю код подтверждения для проверки его позже из его почты по ссылке.
                await _userRepository.SetConfirmAccountCodeAsync(Guid.NewGuid(), userId);

                // Добавляем пользователю бесплатную подписку.
                await _subscriptionRepository.AddUserSubscriptionAsync(userId, SubscriptionTypeEnum.FareRule, 1);
            
                // Отправляем анкету на модерацию.
                await _resumeModerationRepository.AddResumeModerationAsync(profileInfoId);
            }

            // Если пользователь уже существует, то выдаем ему доступ.
            var userCode = await _userRepository.GetUserCodeByVkUserIdAsync(vkUserId);
            var claim = GetIdentityClaimVkUser(vkUserId);
            var token = CreateTokenFactory(claim);

            result.Token = token;
            result.IsSuccess = true;
            result.UserCode = userCode;
            result.VkUserId = vkUserId;

            await tran.CommitAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            await _logger.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод проверяет блокировку пользователя по параметру, который передали.
    /// Поочередно проверяем по почте, номеру телефона.
    /// </summary>
    /// <param name="availableBlockedText">Почта или номер телефона для проверки блокировки.</param>
    /// <param name="isVkAuth">Признак блокировки через ВК.</param>
    private async Task CheckUserBlackListAsync(string availableBlockedText, bool isVkAuth)
    {
        var isBlockedUser = await _accessUserService.CheckBlockedUserAsync(availableBlockedText, isVkAuth);
        
        // Если пользователь заблокирован, выводим предупреждение об этом.
        if (isBlockedUser)
        {
            // Ищем пользователя по почте.
            var userId = await _userRepository.GetUserByEmailAsync(availableBlockedText);

            // Не нашли по почте, пойдем искать по номеру телефона.
            if (userId <= 0)
            {
                userId = await _userRepository.GetUserIdByPhoneNumberAsync(availableBlockedText);
                
                if (userId <= 0)
                {
                    var ex = new NotFoundUserIdByAccountException(availableBlockedText);
                    throw ex;
                }
            }

            var logError = new InvalidOperationException(
                $"Пользователь заблокирован и пытался пройти авторизацию. Пользователь: {availableBlockedText}");
            await _logger.LogErrorAsync(logError);

            throw new InvalidOperationException(
                "Пользователь заблокирован. Причины блокировки можно узнать у тех.поддержки.");
        }
    }

    /// <summary>
    /// Метод парсит строку токена с данными Google аккаунта пользователя.
    /// </summary>
    /// <param name="googleAuthToken">Токен с данными аккаунта.</param>
    /// <returns>Список с результатами после парсинга.</returns>
    private List<Claim> DecodeJwtGoogleData(string googleAuthToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(googleAuthToken);
        var result = jwtSecurityToken.Claims.ToList();

        return result;
    }

    /// <summary>
    /// Метод создает модель с данными аккаунта Google.
    /// </summary>
    /// <param name="claims">Список результатов парсинга.</param>
    /// <returns>Данные аккаунта Google.</returns>
    private UserGoogleInput CreateGoogleDataFactory(List<Claim> claims)
    {
        var result = new UserGoogleInput
        {
            Email =  claims.Find(f => f.Type.Equals("email"))?.Value,
            EmailVerified = Convert.ToBoolean(claims.Find(f => f.Type.Equals("email_verified"))?.Value),
            GivenName = claims.Find(f => f.Type.Equals("given_name"))?.Value,
            FamilyName = claims.Find(f => f.Type.Equals("family_name"))?.Value
        };

        return result;
    }
    
    #endregion
}