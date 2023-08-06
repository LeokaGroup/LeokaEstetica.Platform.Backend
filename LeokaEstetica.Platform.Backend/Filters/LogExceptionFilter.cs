using System.Text;
using LeokaEstetica.Platform.Integrations.Abstractions.Telegram;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LeokaEstetica.Platform.Backend.Filters;

/// <summary>
/// Класс глобального фильтра, который ловит все ошибки приложения и отправляет боту.
/// </summary>
public class LogExceptionFilter : ExceptionFilterAttribute
{
    private readonly ITelegramBotService _telegramBotService;
    private readonly IWebHostEnvironment _env;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="telegramBotService">Сервис телеграм ботов.</param>
    /// <param name="env">Окружение.</param>
    public LogExceptionFilter(ITelegramBotService telegramBotService,
        IWebHostEnvironment env)
    {
        _telegramBotService = telegramBotService;
        _env = env;
    }

    /// <summary>
    /// Метод обработки исключения.
    /// </summary>
    /// <param name="context">Контекст исключения.</param>
    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        await base.OnExceptionAsync(context);
        
        var environment = string.Empty;

        if (_env.IsDevelopment())
        {
            environment = "[Develop]";
        }
        
        else if (_env.IsStaging())
        {
            environment = "[Test]";
        }
        
        else if (_env.IsProduction())
        {
            environment = "[Production]";
        }

        var errorMessage = new StringBuilder();
        errorMessage.Append(environment);
        errorMessage.AppendLine("ErrorMessage: ");
        errorMessage.AppendLine(context.Exception.Message);
        errorMessage.AppendLine("Guid: " + Guid.NewGuid());
        errorMessage.AppendLine("StackTrace: ");
        errorMessage.AppendLine(context.Exception.StackTrace);
        
        // Отправляем информацию об исключении в канал телеграма.
        await _telegramBotService.SendErrorMessageAsync(errorMessage.ToString());
    }
}