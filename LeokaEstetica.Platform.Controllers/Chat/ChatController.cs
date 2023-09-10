using FluentValidation.Results;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Controllers.Validators.Chat;
using LeokaEstetica.Platform.Messaging.Abstractions.Chat;
using LeokaEstetica.Platform.Models.Dto.Chat.Input;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Chat;

/// <summary>
/// TODO: Этот контроллер удалим когда переведем все методы чатов на сокеты, он будет не нужен.
/// Контроллер для работы с чатами.
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
            dialogInput.DiscussionTypeId);

        return result;
    }

    /// <summary>
    /// Метод получает список диалогов для ЛК.
    /// </summary>
    /// <returns>Список диалогов.</returns>
    [HttpGet]
    [Route("profile-messages")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ProfileDialogOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<ProfileDialogOutput>> GetProfileDialogsAsync()
    {
        var result = await _chatService.GetProfileDialogsAsync(GetUserName());

        return result;
    }
}