using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LeokaEstetica.Platform.Core.Filters;

/// <summary>
/// Фильтр авторизации.
/// </summary>
public class AuthFilter : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Проверяет, авторизован ли пользователь.
        // Также это не должен быть роут меню профиля.
        // Потому что фильтр глобальный, а есть места, где он не нужен.
        if (context.HttpContext.User.Identity is not null 
            && !context.HttpContext.User.Identity.IsAuthenticated
            && !new[] {"GetProfileMenuItems"}.Contains(context.RouteData.Values["action"]))
        {
            context.Result =  new ForbidResult();
        }
    }
}