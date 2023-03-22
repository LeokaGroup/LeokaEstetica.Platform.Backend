using LeokaEstetica.Platform.Core.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Base;

/// <summary>
/// Базовый контроллер для всех контроллеров приложения.
/// </summary>
public class BaseController : ControllerBase
{
    /// <summary>
    /// Метод получит имя текущего пользователя.
    /// </summary>
    /// <returns>Логин пользователя.</returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    protected string GetUserName()
    {
        // Запишет логин в куки и вернет фронту.
        if (!HttpContext.Request.Cookies.ContainsKey("name"))
        {
            HttpContext.Response.Cookies.Append("user", HttpContext?.User?.Identity?.Name
                                                        ?? GetLoginFromCookie()
                                                        ?? GetLoginFromHeaders());
        }

        return HttpContext?.User?.Identity?.Name ?? GetLoginFromCookie() ?? GetLoginFromHeaders();
    }
    
    /// <summary>
    /// Метод вернет логин пользователя из куки.
    /// </summary>  
    /// <returns>Логин пользователя.</returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    private string GetLoginFromCookie()
    {
        return HttpContext.Request.Cookies["user"];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    private string GetLoginFromHeaders()
    {
        if (HttpContext.Request.Headers.TryGetValue("c_dt", out _))
        {
            // Внутри есть элементы.
            // [0] - ConnectionId.
            // [1] - Email.
            return HttpContext.Request.Headers.TryGet("c_dt").ToString().Split(":")[1];
        }

        return null;
    }
}