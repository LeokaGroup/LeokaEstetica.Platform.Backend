using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Models.Dto.Input.User;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Services.Abstractions.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    
    public UserController(IUserService userService)
    {
        _userService = userService;
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
        var result = await _userService.CreateUserAsync(userSignUpInput.Password, userSignUpInput.Email);

        return result;
    }

    /// <summary>
    /// Метод подтверждает аккаунт пользователя по коду, который ранее был отправлен пользователю на почту и записан в БД.
    /// </summary>
    /// <param name="code">Код подтверждения.</param>
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
}