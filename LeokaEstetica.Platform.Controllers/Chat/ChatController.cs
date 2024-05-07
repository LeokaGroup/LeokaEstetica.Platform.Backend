using FluentValidation.Results;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Abstractions.Messaging.Chat;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Messaging.Validators;
using LeokaEstetica.Platform.Models.Dto.Chat.Input;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Enum = System.Enum;

namespace LeokaEstetica.Platform.Controllers.Chat;

/// <summary>
/// Контроллер для работы с чатами.
/// P.S: Большая часть логики с чатами работает на сокетах и находится в хабе ChatHub.
/// </summary>
[AuthFilter]
[ApiController]
[Route("chat")]
public class ChatController : BaseController
{
    private readonly IChatService _chatService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="chatService">Чат сервиса.</param>
    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    /// <summary>
    /// Метод создает диалог для написания владельцу проекта.
    /// Если такой диалог уже создан с текущим юзером и владельцем проекта,
    /// то ничего не происходит и диалог считается пустым для начала общения.
    /// </summary>
    /// <param name="dialogInput">Входная модель.</param>
    /// <returns>Данные диалога.</returns>
    [HttpPost]
    [Route("write")]
    [ProducesResponseType(200, Type = typeof(DialogResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<DialogResultOutput> WriteProjectDialogOwnerAsync([FromBody] DialogInput dialogInput)
    {
        var result = new DialogResultOutput { Errors = new List<ValidationFailure>() };
        var validator = await new GetDialogValidator().ValidateAsync(dialogInput);

        if (validator.Errors.Any())
        {
            return result;
        }

        Enum.TryParse(dialogInput.DiscussionType, out DiscussionTypeEnum discussionType);
        
        result = await _chatService.WriteProjectDialogOwnerAsync(discussionType, GetUserName(),
            dialogInput.DiscussionTypeId, GetTokenFromHeader());

        return result;
    }
}