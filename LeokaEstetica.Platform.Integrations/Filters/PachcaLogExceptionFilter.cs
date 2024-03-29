using LeokaEstetica.Platform.Integrations.Abstractions.Pachca;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LeokaEstetica.Platform.Integrations.Filters;

/// <summary>
/// Класс глобального фильтра, который ловит все ошибки приложения и отправляет боту.
/// </summary>
public class PachcaLogExceptionFilter : ExceptionFilterAttribute
{
    private readonly IPachcaService _pachcaService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pachcaService">Сервис мессенджера пачки.</param>
    public PachcaLogExceptionFilter(IPachcaService pachcaService)
    {
        _pachcaService = pachcaService;
    }

    /// <summary>
    /// Метод обработки исключения.
    /// </summary>
    /// <param name="context">Контекст исключения.</param>
    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        await base.OnExceptionAsync(context);

        // Отправляем информацию об исключении в канал телеграма.
        await _pachcaService.SendNotificationErrorAsync(context.Exception);
    }
}