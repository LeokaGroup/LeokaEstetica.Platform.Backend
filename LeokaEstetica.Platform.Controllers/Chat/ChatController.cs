using FluentValidation.Results;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Validators.Chat;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Messaging.Abstractions.Chat;
using LeokaEstetica.Platform.Messaging.Models.Chat.Output;
using LeokaEstetica.Platform.Models.Dto.Chat.Input;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Chat;

/// <summary>
/// Контроллер для работы с чатами.
/// </summary>
[AuthFilter]
[ApiController]
[Route("chat")]
public class ChatController : BaseController
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    /// <summary>
    /// Метод получает диалог или создает новый и возвращает его.
    /// </summary>
    /// <param name="dialogInput">Входная модель.</param>
    /// <returns>Данные диалога.</returns>
    [HttpGet]
    [Route("dialog")]
    [ProducesResponseType(200, Type = typeof(DialogResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<DialogResultOutput> GetDialogAsync([FromQuery] DialogInput dialogInput)
    {
        var result = new DialogResultOutput { Errors = new List<ValidationFailure>() };
        var validator = await new GetDialogValidator().ValidateAsync(dialogInput);

        if (validator.Errors.Any())
        {
            return result;
        }

        result = await _chatService.GetDialogAsync(dialogInput.DialogId, dialogInput.DiscussionType, GetUserName(),
            dialogInput.DiscussionTypeId);

        return result;
    }
}