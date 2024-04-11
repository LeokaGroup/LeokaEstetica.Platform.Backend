using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LeokaEstetica.Platform.Integrations.Filters;

/// <summary>
/// Класс глобального фильтра, который ловит все ошибки приложения и отправляет боту.
/// </summary>
public class DiscordLogExceptionFilter : ExceptionFilterAttribute
{
    private readonly IDiscordService _discordService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pachcaService">Сервис мессенджера пачки.</param>
    public DiscordLogExceptionFilter(IDiscordService discordService)
    {
        _discordService = discordService;
    }

    /// <summary>
    /// Метод обработки исключения.
    /// </summary>
    /// <param name="context">Контекст исключения.</param>
    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        await base.OnExceptionAsync(context);

        // Отправляем информацию об исключении в дискорд.
        await _discordService.SendNotificationErrorAsync(context.Exception);
    }
}