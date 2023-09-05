using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using AutoMapper;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Access.Helpers;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.AvailableLimits;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Resume;
using LeokaEstetica.Platform.Database.Abstractions.Profile;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Models.Dto.Input.User;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Entities.User;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Redis.Abstractions.User;
using LeokaEstetica.Platform.Services.Abstractions.User;
using LeokaEstetica.Platform.Services.Consts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.User;

/// <summary>
/// Класс реализует методы сервиса пользователей.
/// </summary>
internal sealed class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IMailingsService _mailingsService;
    private readonly PgContext _pgContext;
    private readonly IProfileRepository _profileRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IResumeModerationRepository _resumeModerationRepository;
    private readonly IAccessUserService _accessUserService;
    private readonly IUserRedisService _userRedisService;
    private readonly IFareRuleRepository _fareRuleRepository;
    private readonly IAccessUserNotificationsService _accessUserNotificationsService;
    private readonly INotificationsService _notificationsService;
    private readonly IAvailableLimitsRepository _availableLimitsRepository;
    private readonly IGlobalConfigRepository _globalConfigRepository;

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
    /// <param name="fareRuleRepository">Репозиторий тарифов.</param>
    /// <param name="accessUserNotificationsService">Сервис уведомлений доступа пользователей.</param>
    /// <param name="notificationsService">Сервис уведомлений.</param>
    /// <param name="availableLimitsRepository">Репозиторий лимитов.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    public UserService(ILogger<UserService> logger, 
        IUserRepository userRepository, 
        IMapper mapper, 
        IMailingsService mailingsService, 
        PgContext pgContext, 
        IProfileRepository profileRepository, 
        ISubscriptionRepository subscriptionRepository, 
        IResumeModerationRepository resumeModerationRepository, 
        IAccessUserService accessUserService, 
        IUserRedisService userRedisService, 
        IFareRuleRepository fareRuleRepository,
        IAccessUserNotificationsService accessUserNotificationsService,
        INotificationsService notificationsService,
        IAvailableLimitsRepository availableLimitsRepository,
        IGlobalConfigRepository globalConfigRepository)
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
        _userRedisService = userRedisService;
        _fareRuleRepository = fareRuleRepository;
        _accessUserNotificationsService = accessUserNotificationsService;
        _notificationsService = notificationsService;
        _availableLimitsRepository = availableLimitsRepository;
        _globalConfigRepository = globalConfigRepository;
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

            if (result.Errors.Any())
            {
                return result;
            }

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
            _logger.LogError(ex, ex.Message);
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
            result.Errors = new List<ValidationFailure>
            {
                new() { ErrorMessage = $"Пользователь с Email {email} уже зарегистрирован в системе." }
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
            DateRegister = DateTime.UtcNow,
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
            _logger.LogCritical(ex, ex.Message);
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
            _logger.LogError(ex, ex.Message);
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
            var IfExistsUserEmail = await _userRepository.CheckUserByEmailAsync(email);
            var result = new UserSignInOutput { Errors = new List<ValidationFailure>() };

            // Если нет такой почты в системе.
            if (!IfExistsUserEmail)
            {
                result.Errors.Add(new ValidationFailure
                {
                    ErrorCode = "500",
                    ErrorMessage = string.Format(ValidationConsts.NOT_VALID_EMAIL, email)
                });
                ;
                var ex = new UnauthorizedAccessException(string.Format(ValidationConsts.NOT_VALID_EMAIL, email));
                _logger.LogError(string.Format(ValidationConsts.NOT_VALID_EMAIL, email), ex);

                return result;
            }
            
            var passwordHash = await _userRepository.GetPasswordHashByEmailAsync(email);

            if (passwordHash is null)
            {
                throw new InvalidOperationException($"Хэш пароль не удалось получить для пользователя {email}");
            }

            var checkPassword = HashHelper.VerifyHashedPassword(passwordHash, password);

            // Если пароль некорректный.
            if (!checkPassword)
            {
                result.Errors.Add(new ValidationFailure
                {
                    ErrorCode = "500",
                    ErrorMessage = ValidationConsts.NOT_VALID_PASSWORD
                });

                var errMsg = $"Пользователь {email} не прошел проверку по паролю.";
                var ex = new UnauthorizedAccessException(errMsg);
                _logger.LogError(errMsg, ex);

                return result;
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

            var claim = await _accessUserService.GetIdentityClaimAsync(email);
            var token = await _accessUserService.CreateTokenFactoryAsync(claim);
            
            result.Email = email;
            result.Token = token;
            result.IsSuccess = true;
            result.UserCode = userCode;

            var userId = await _userRepository.GetUserIdByEmailAsync(email);

            await IfDisableUserSubscriptionAsync(userId);

            // Записываем токен пользователя в кэш.
            await _userRedisService.AddUserTokenAndUserIdCacheAsync(userId, result.Token);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

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
            var email = googleUserData.Email;
            
            // Првоеряем, существует ли такой пользователь в системе.
            var isUserExists = await _userRepository.CheckUserByEmailAsync(email);
            
            // Пользователя нет, регистрируем его и выдаем доступ.
            if (!isUserExists)
            {
                var userModel = CreateSignUpUserModel(string.Empty, email);
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
            var userCode = await _userRepository.GetUserCodeByEmailAsync(email);
            var claim = await _accessUserService.GetIdentityClaimAsync(email);
            var token = await _accessUserService.CreateTokenFactoryAsync(claim);

            var result = new UserSignInOutput
            {
                Errors = new List<ValidationFailure>(),
                Email = email,
                Token = token,
                IsSuccess = true,
                UserCode = userCode
            };

            await tran.CommitAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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
            var claim = await _accessUserService.GetIdentityClaimVkUserAsync(vkUserId);
            var token = await _accessUserService.CreateTokenFactoryAsync(claim);

            result.Token = token;
            result.IsSuccess = true;
            result.UserCode = userCode;
            result.VkUserId = vkUserId;

            await tran.CommitAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод отправляет код пользователю на почту для восстановления пароля.
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>
    /// <returns>Признак успешного прохождения проверки.</returns>
    /// </summary>
    public async Task<bool> SendCodeRestorePasswordAsync(string account, string token)
    {
        try
        {
            // Проверяем пользователя на блокировку.
            var isBlocked = await _accessUserService.CheckBlockedUserAsync(account, false);

            // Пользователь заблокирован, не даем ничего делать.
            if (isBlocked)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    await _accessUserNotificationsService.SendNotificationWarningBlockedUserProfileAsync("Внимание",
                        "Невозможно восстановить пароль. Пользователь заблокирован. Причины блокировки можно узнать у тех.поддержки.",
                        NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);   
                }

                throw new InvalidOperationException(
                    "Пользователь заблокирован. Причины блокировки можно узнать у тех.поддержки.");
            }
            
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var guid = Guid.NewGuid();
            
            // Добавляем данные для восстановления в кэш.
            await _userRedisService.AddRestoreUserDataCacheAsync(guid, userId);
            
            // Отправляем ссылку для восстановления пароля пользователю на почту.
            await _mailingsService.SendLinkRestorePasswordAsync(account, guid);

            if (!string.IsNullOrEmpty(token))
            {
                await _accessUserNotificationsService.SendNotificationSuccessLinkRestoreUserPasswordAsync("Все хорошо",
                    "Дальнейшие инструкции по восстановлению пароля направлены Вам на почту.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);   
            }

            return true;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод проверяет доступ к восстановлению пароля пользователя.
    /// </summary>
    /// <param name="publicKey">Публичный код, который ранее высалался на почту пользователю.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Признак успешного прохождения проверки.</returns>
    public async Task<bool> CheckRestorePasswordAsync(Guid publicKey, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var guid = await _userRedisService.GetRestoreUserDataCacheAsync(userId);

            if (!guid)
            {
                throw new InvalidOperationException(
                    $"Не удалось проверить код {publicKey} для восстановления пароля пользователя.");
            }

            return true;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод запускает восстановление пароля пользователя.
    /// </summary>
    /// <param name="password">Новый пароль.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>
    public async Task RestoreUserPasswordAsync(string password, string account, string token)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var passwordHash = HashHelper.HashPassword(password);
            await _userRepository.RestoreUserPasswordAsync(passwordHash, userId);
            
            if (!string.IsNullOrEmpty(token))
            {
                await _notificationsService.SendNotifySuccessRestoreUserPasswordAsync("Все хорошо",
                    "Пароль успешно изменен.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);   
            }
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает конфигурацию дял аутентификации через разных провайдеров в зависимости от среды окружения.
    /// </summary>
    /// <returns>Данные с ссылками для аутентификации через провайдеров.</returns>
    public async Task<AuthProviderConfigOutput> GetAuthProviderConfigAsync()
    {
        try
        {
            var vkReference = await _globalConfigRepository.GetValueByKeyAsync<string>(
                GlobalConfigKeys.AuthProviderReference.AUTH_PROVIDER_REFERENCE_VK);

            if (string.IsNullOrEmpty(vkReference))
            {
                throw new InvalidOperationException(
                    "Не удалось получить ссылку для аутентификации через провайдер VK.");
            }
        
            var vkRedirect = await _globalConfigRepository.GetValueByKeyAsync<string>(
                GlobalConfigKeys.AuthProviderReference.AUTH_PROVIDER_REDIRECT_REFERENCE_VK);
            
            if (string.IsNullOrEmpty(vkRedirect))
            {
                throw new InvalidOperationException(
                    "Не удалось получить ссылку для редиректа после успешной аутентификации через провайдер VK.");
            }
            
            var googleReference = await _globalConfigRepository.GetValueByKeyAsync<string>(
                GlobalConfigKeys.AuthProviderReference.AUTH_PROVIDER_REFERENCE_GOOGLE);
            
            if (string.IsNullOrEmpty(googleReference))
            {
                throw new InvalidOperationException(
                    "Не удалось получить ссылку для аутентификации через провайдер Google.");
            }
            
            var googleRedirect = await _globalConfigRepository.GetValueByKeyAsync<string>(
                GlobalConfigKeys.AuthProviderReference.AUTH_PROVIDER_REDIRECT_REFERENCE_GOOGLE);
            
            if (string.IsNullOrEmpty(googleRedirect))
            {
                throw new InvalidOperationException(
                    "Не удалось получить ссылку для редиректа после успешной аутентификации через провайдер Google.");
            }

            var result = new AuthProviderConfigOutput
            {
                VkReference = vkReference,
                VkRedirectReference = vkRedirect,
                GoogleReference = googleReference,
                GoogleRedirectReference = googleRedirect
            };

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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
            _logger.LogError(logError, logError.Message);

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

    /// <summary>
    /// Метод отключает подписку пользователя, если срок ее истек.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    private async Task IfDisableUserSubscriptionAsync(long userId)
    {
        // Проверяем активность подписки пользователя, если она платная.
        var subscription = await _subscriptionRepository.GetUserSubscriptionAsync(userId);
        
        if (subscription is null)
        {
            _logger.LogError($"Не удалось получить подписку. UserId: {userId}");

            // Сбрасываем подписку пользователя на бесплатный тариф.
            await _subscriptionRepository.AutoDefaultUserSubscriptionAsync(userId);

            _logger.LogInformation(
                "Автоматический сброс подписки пользователя на бесплатный тариф по причине не продления подписки." +
                $" UserId: {userId}");

            await _availableLimitsRepository.RestrictionFreeLimitsAsync(userId);

            _logger.LogInformation(
                "Автоматическая отправка проектов и вакансий в архив по причине сброса подписки пользователя" +
                " на бесплатный тариф по причине не продления подписки." +
                $" UserId: {userId}");

            return;
        }
            
        // Получаем тариф.
        var fareRule = await _fareRuleRepository.GetByIdAsync(subscription.ObjectId);

        // Бесплатный тариф нас не интересует.
        if (!fareRule.IsFree)
        {
            var subscriptionId = subscription.SubscriptionId;
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionBySubscriptionIdAsync(
                subscriptionId, userId);

            if (userSubscription is null)
            {
                throw new InvalidOperationException("Не удалось получить подписку пользователя." +
                                                    $"UserId: {userId}." +
                                                    $"SubscriptionId: {subscriptionId}");
            }

            var dates = await _userRepository.GetUserSubscriptionUsedDateAsync(userId);

            // Отключаем пользователю подписку.
            if (dates.EndDate < DateTime.UtcNow)
            {
                await _subscriptionRepository.DisableUserSubscriptionAsync(userId);
            }
        }
    }
    
    #endregion
}