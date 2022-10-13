using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Core.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.User;

/// <summary>
/// Контроллер профиля пользователя.
/// </summary>
[AuthFilter]
[ApiController]
[Route("profile")]
public class ProfileController : BaseController
{
    
}