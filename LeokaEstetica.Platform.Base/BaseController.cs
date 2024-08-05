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
        var user = HttpContext.User?.Identity?.Name ?? GetLoginFromCookie();

        if (user is null)
        {
            return null;
        }
        
        // Запишет логин в куки и вернет фронту.
        if (!HttpContext.Request.Cookies.ContainsKey("name"))
        {
            HttpContext.Response.Cookies.Append("user", user);
        }

        return user;
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
    /// Метод вернет токен пользователя из хидера и обрезает строку для получения нужной части.
    /// </summary>  
    /// <returns>Токен пользователя.</returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    public string CreateTokenFromHeader()
    {
        var token = HttpContext.Request.Headers.TryGet("Authorization").ToString();

        if (token.Contains("Bearer"))
        {
            return HttpContext.Request.Headers.TryGet("Authorization").ToString().Substring(7);
        }

        return GetTokenFromHeader();
    }
    
    /// <summary>
    /// Метод вернет токен пользователя из хидера в исходном виде.
    /// </summary>  
    /// <returns>Токен пользователя.</returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    public string GetTokenFromHeader()
    {
        var token = HttpContext.Request.Headers.TryGet("Authorization").ToString();
        
        if (!token.Contains("Bearer"))
        {
            return HttpContext.Request.Headers.TryGet("Authorization").ToString();
        }
        
        return CreateTokenFromHeader();
    }
}