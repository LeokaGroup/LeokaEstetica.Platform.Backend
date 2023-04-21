using FluentValidation.Results;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Controllers.Filters;
using LeokaEstetica.Platform.Controllers.Validators.Chat;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Messaging.Abstractions.Chat;
using LeokaEstetica.Platform.Messaging.Models.Chat.Output;
using LeokaEstetica.Platform.Models.Dto.Chat.Input;
using LeokaEstetica.Platform.Models.Dto.Chat.Output;
using LeokaEstetica.Platform.Models.Enums;
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
    private readonly ILogService _logService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="chatService">Чат сервиса.</param>
    /// <param name="logService">Сервис логирования</param>
    public ChatController(IChatService chatService,
                          ILogService logService)
    {
        _chatService = chatService;
        _logService = logService;
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

        try
        {
            var result = new DialogResultOutput { Errors = new List<ValidationFailure>() };
            var validator = await new GetDialogValidator().ValidateAsync(dialogInput);

            if (validator.Errors.Any())
            {
                return result;
            }

            Enum.TryParse(dialogInput.DiscussionType, out DiscussionTypeEnum discussionType);
            result = await _chatService.GetDialogAsync(dialogInput.DialogId, discussionType, GetUserName(),
                dialogInput.DiscussionTypeId);

            return result;
        }
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список диалогов.
    /// </summary>
    /// <returns>Список диалогов.</returns>
    [HttpGet]
    [Route("dialogs")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<DialogOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<DialogOutput>> GetDialogsAsync()
    {
        var result = await _chatService.GetDialogsAsync(GetUserName());

        return result;
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
        try
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
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод отправляет сообщение.
    /// </summary>
    /// <param name="messageInput">Входная модель.</param>
    /// <returns>Выходная модель.</returns>
    [HttpPost]
    [Route("message")]
    [ProducesResponseType(200, Type = typeof(DialogResultOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<DialogResultOutput> SendMessageAsync([FromBody] MessageInput messageInput)
    {
        var result = await _chatService.SendMessageAsync(messageInput.Message, messageInput.DialogId, GetUserName());

        return result;
    }
}