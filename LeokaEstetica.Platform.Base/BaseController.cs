using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Base;

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
            HttpContext.Response.Cookies.Append("user", HttpContext?.User?.Identity?.Name ?? GetLoginFromCookie());
        }

        return HttpContext?.User?.Identity?.Name ?? GetLoginFromCookie();
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
}