using System.Web.Http.ExceptionHandling;
using LeokaEstetica.Platform.Controllers.Middlewares;
using Microsoft.Extensions.Configuration;

namespace LeokaEstetica.Platform.Controllers.Factories;

/// <summary>
/// Класс факторки Api-исключений.
/// </summary>
public static class CreateErrorMessageFactory
{
    /// <summary>
    /// Среды окружения.
    /// </summary>
    private static readonly HashSet<string> _environments = new() { "Development, Staging, Production" };

    /// <summary>
    /// Метод создает ApiError объект в нужном виде.
    /// </summary>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="configuration">Конфигурация.</param>
    /// <returns>ApiError объект.</returns>
    public static ApiErrorHandler.ErrorMessage Create(ExceptionHandlerContext context, IConfiguration configuration)
    {
        // TODO: Надо будет доработать, чтобы у нас всегда был гуид исключения для более удобного поиска в БД и графане.
        // var isApiErrorMode = service.GetValueByKey<bool>(GlobalConfigKeys.API_ERROR_FILTER_ENABLED_MODE);
        // var traceId = HttpContext.Current.GetOwinContext().Request.Get<Guid>("TraceId");

        // Для dev и test сред нам нужно показывать исключение.
        if (_environments.Contains(configuration["Environment"]))
        {
            return new ApiErrorHandler.ErrorMessage(context.Exception.ToString())
            {
                ExceptionType = context.Exception.GetType().Name
                // TraceId = traceId
            };
        }

        return new ApiErrorHandler.ErrorMessage("Internal server error")
        {
            ExceptionType = null
            // TraceId = traceId
        };
    }
}