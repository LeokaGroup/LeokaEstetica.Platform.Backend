using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Integrations.Abstractions.Telegram;
using LeokaEstetica.Platform.Models.Dto.Output.Integration.Telegram;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Telegram;

/// <summary>
/// Контроллер телеграма.
/// </summary>
[ApiController]
[Route("telegram")]
public class TelegramController : BaseController
{
    private readonly ITelegramService _telegramService;
    
    /// <summary>
    /// Конструктор.
    /// <param name="telegramService">Сервис телеграма.</param>
    /// </summary>
    public TelegramController(ITelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    /// <summary>
    /// Метод создает ссылку для приглашения пользователя в канал уведомлений телеграмма.
    /// </summary>
    /// <returns>Строка приглашения.</returns>
    [HttpGet]
    [Route("invite")]
    [ProducesResponseType(200, Type = typeof(CreateInviteLInkOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<CreateInviteLInkOutput> CreateNotificationsChanelInviteLinkAsync()
    {
        var result = await _telegramService.CreateNotificationsChanelInviteLinkAsync();

        return result;
    }
}