using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LeokaEstetica.Platform.Controllers.Filters;

/// <summary>
/// Фильтр авторизации.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AuthFilter : Attribute, IAuthorizationFilter
{
    /// <summary>
    /// Список допустимых апи, котороые не надо проверять.
    /// </summary>
    private static readonly HashSet<string> _allowActions = new()
    {
        "SignIn",
        "CreateUser",
        "GetFareRules", // Правила тарифов.
        "AddConnectionIdCache",
        "GetTicketCategories", // Категории тикетов.
        "CreateTicket",
        "GetLastProjectComments",
        "GetNewUsers",
        "CreateWisheOffer",
        "GetAuthProviderConfig" // Получение конфига аутентификации провайдеров.
    };
    
    /// <summary>
    /// Метод проверки авторизации.
    /// </summary>
    /// <param name="context">Контекст.</param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Проверяет, авторизован ли пользователь.
        // У этой проверки имеются исключения на определенные ендпоинты.
        if (context.HttpContext.User.Identity is not null
            && !context.HttpContext.User.Identity.IsAuthenticated
            && !_allowActions.Contains(context.RouteData.Values["action"]))
        {
            context.Result = new ForbidResult();
        }
    }
}