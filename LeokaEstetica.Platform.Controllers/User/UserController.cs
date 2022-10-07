using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Models.Dto.Input.User;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Services.Abstractions.User;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.User;

/// <summary>
/// Контроллер работы с пользователями.
/// </summary>
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
    [HttpPost]
    [Route("signup")]
    public async Task<UserSignUpOutput> CreateUserAsync([FromBody] UserSignUpInput userSignUpInput)
    {
        var result = await _userService.CreateUserAsync(userSignUpInput.Email, userSignUpInput.Password);

        return result;
    }
}