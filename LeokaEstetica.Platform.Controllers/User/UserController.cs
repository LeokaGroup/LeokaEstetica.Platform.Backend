using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Abstractions.Services.Validation;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Controllers.Validators.User;
using LeokaEstetica.Platform.Models.Dto.Input.User;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Services.Abstractions.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Controllers.User;

/// <summary>
/// Контроллер работы с пользователями.
/// </summary>
[AuthFilter]
[ApiController]
[Route("user")]
public class UserController : BaseController
{
    private readonly IUserService _userService;
    private readonly IValidationExcludeErrorsService _validationExcludeErrorsService;
    private readonly ILogger<UserController> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userService">Сервис пользователя.</param>
    /// <param name="validationExcludeErrorsService">Сервис исключения ошибок, которые не надо проверять.</param>
    /// <param name="logger">Логгер.</param>
    public UserController(IUserService userService, 
        IValidationExcludeErrorsService validationExcludeErrorsService,
        ILogger<UserController> logger)
    {
        _userService = userService;
        _validationExcludeErrorsService = validationExcludeErrorsService;
        _logger = logger;
    }

    /// <summary>
    /// Метод создает нового пользователя.
    /// </summary>
    /// <param name="userSignUpInput">Входная модель.</param>
    /// <returns>Данные пользователя.</returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("signup")]
    [ProducesResponseType(200, Type = typeof(UserSignUpOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UserSignUpOutput> CreateUserAsync([FromBody] UserSignUpInput userSignUpInput)
    {
        var result = new UserSignUpOutput();
        var validator = await new CreateUserValidator().ValidateAsync(userSignUpInput);

        if (validator.Errors.Any())
        {
            result.Errors = await _validationExcludeErrorsService.ExcludeAsync(validator.Errors);

            return result;
        }
        
        result = await _userService.CreateUserAsync(userSignUpInput.Password, userSignUpInput.Email);

        return result;
    }

    /// <summary>
    /// Метод подтверждает аккаунт пользователя по коду, который ранее был отправлен пользователю на почту и записан в БД.
    /// </summary>
    /// <param name="confirmAccountInput">Входная модель.</param>
    /// <returns>Статус подтверждения.</returns>
    [AllowAnonymous]
    [HttpPatch]
    [Route("account/confirm")]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<bool> ConfirmAccountAsync([FromBody] ConfirmAccountInput confirmAccountInput)
    {
        var result = await _userService.ConfirmAccountAsync(confirmAccountInput.ConfirmAccountCode);

        return result;
    }

    /// <summary>
    /// Метод авторизует пользователя.
    /// </summary>
    /// <param name="userSignInInput">Входная модель.</param>
    /// <returns>Данные авторизации.</returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("signin")]
    [ProducesResponseType(200, Type = typeof(UserSignInOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UserSignInOutput> SignInAsync([FromBody] UserSignInInput userSignInInput)
    {
        var result = new UserSignInOutput();
        var validator = await new SignInValidator().ValidateAsync(userSignInInput);

        if (validator.Errors.Any())
        {
            result.Errors = await _validationExcludeErrorsService.ExcludeAsync(validator.Errors);

            return result;
        }
        
        result = await _userService.SignInAsync(userSignInInput.Email, userSignInInput.Password);

        return result;
    }

    /// <summary>
    /// Метод обновляет токен.
    /// </summary>
    /// <returns>Новые данные авторизации.</returns>
    [HttpPost]
    [Route("token")]
    [ProducesResponseType(200, Type = typeof(UserSignInOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UserSignInOutput> RefreshTokenAsync()
    {
        var result = await _userService.RefreshTokenAsync(GetUserName());

        return result;
    }

    /// <summary>
    /// Метод авторизации через Google. Если аккаунт не зарегистрирован в системе,
    /// то создаем также аккаунт используя данные аккаунта Google пользователя.
    /// </summary>
    /// <param name="userSignInGoogleInput">Входная модель.</param>
    /// <returns>Данные пользователя.</returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("signin-google")]
    [ProducesResponseType(200, Type = typeof(UserSignInOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UserSignInOutput> SignInAsync([FromBody] UserSignInGoogleInput userSignInGoogleInput)
    {
        var result = new UserSignInOutput();
        var validator = await new SignInGoogleValidator().ValidateAsync(userSignInGoogleInput);

        if (validator.Errors.Any())
        {
            result.Errors = validator.Errors;

            return result;
        }

        result = await _userService.SignInAsync(userSignInGoogleInput.GoogleAuthToken);

        return result;
    }

    /// <summary>
    /// Метод авторизации через ВК. Если аккаунт не зарегистрирован в системе,
    /// то создаем также аккаунт используя данные аккаунта ВК пользователя.
    /// </summary>
    /// <param name="userSignInVkInput">Входная модель.</param>
    /// <returns>Данные пользователя.</returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("signin-vk")]
    [ProducesResponseType(200, Type = typeof(UserSignInOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<UserSignInOutput> SignInAsync([FromBody] UserSignInVkInput userSignInVkInput)
    {
        var result = new UserSignInOutput();
        var validator = await new SignInVkValidator().ValidateAsync(userSignInVkInput);

        if (validator.Errors.Any())
        {
            result.Errors = validator.Errors;

            return result;
        }

        result = await _userService.SignInAsync(userSignInVkInput.VkUserId, userSignInVkInput.FirstName,
            userSignInVkInput.LastName);

        return result;
    }

    /// <summary>
    /// Метод отправляет код пользователю на почту для восстановления пароля.
    /// <returns>Признак успешного прохождения проверки.</returns>
    /// </summary>
    [HttpPost]
    [Route("pre-restore")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<bool> SendCodeRestorePasswordAsync([FromBody] PreRestorePasswordInput preRestorePasswordInput)
    {
        var result = await _userService.SendCodeRestorePasswordAsync(preRestorePasswordInput.Account,
            GetTokenFromHeader());

        return result;
    }

    /// <summary>
    /// Метод проверяет доступ к восстановлению пароля пользователя.
    /// </summary>
    /// <param name="publicKey">Публичный код, который ранее высалался на почту пользователю.</param>
    /// <returns>Признак успешного прохождения проверки.</returns>
    [HttpGet]
    [Route("restore/check")]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<bool> CheckRestorePasswordAsync([FromQuery] Guid publicKey)
    {
        var validator = await new CheckRestorePasswordValidator().ValidateAsync(publicKey);

        if (validator.Errors.Any())
        {
            _logger.LogError("Передали невалидный публичный код при восстановлении пароля.");
            
            return false;
        }

        var result = await _userService.CheckRestorePasswordAsync(publicKey, GetUserName());

        return result;
    }

    /// <summary>
    /// Метод запускает восстановление пароля пользователя.
    /// </summary>
    /// <param name="restorePasswordInput">Входная модель.</param>
    [HttpPatch]
    [Route("restore")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task RestoreUserPasswordAsync([FromBody] RestorePasswordInput restorePasswordInput)
    {
        var validator = await new RestorePasswordValidator().ValidateAsync(restorePasswordInput.RestorePassword);

        if (validator.Errors.Any())
        {
            var ex = new InvalidOperationException("Передали невалидный пароль при восстановлении пароля.");
            _logger.LogError(ex.Message);

            throw ex;
        }

        await _userService.RestoreUserPasswordAsync(restorePasswordInput.RestorePassword, GetUserName(),
            GetTokenFromHeader());
    }
}