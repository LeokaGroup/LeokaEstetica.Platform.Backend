using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Communications.Models.Output;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Communications.Controllers;

/// <summary>
/// Контроллер элементов меню чата.
/// </summary>
[AuthFilter]
[ApiController]
[Route("communications/menu")]
public class MenuController : BaseController
{
    private readonly IChatCommunicationsMenuService _chatCommunicationsMenuService;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="chatCommunicationsMenuService">Сервис элементов меню чата.</param>
    public MenuController(IChatCommunicationsMenuService chatCommunicationsMenuService)
    {
        _chatCommunicationsMenuService = chatCommunicationsMenuService;
    }

    /// <summary>
    /// Метод получает элементы меню для групп объектов чата.
    /// </summary>
    /// <returns>Элементы меню для групп объектов чата.</returns>
    [HttpGet]
    [Route("group-objects-menu")]
    [ProducesResponseType(200, Type = typeof(GroupObjectMenuOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<GroupObjectMenuOutput> GetGroupObjectMenuItemsAsync()
    {
        var result = await _chatCommunicationsMenuService.GetGroupObjectMenuItemsAsync();

        return result;
    }
    
    /// <summary>
    /// Метод получает элементы меню группировок диалогов чата.
    /// </summary>
    /// <returns>Элементы меню группировок диалогов чата.</returns>
    [HttpGet]
    [Route("dialog-group-menu")]
    [ProducesResponseType(200, Type = typeof(DialogGroupMenuOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<DialogGroupMenuOutput> GetDialogGroupMenuItemsAsync()
    {
        var result = await _chatCommunicationsMenuService.GetDialogGroupMenuItemsAsync();

        return result;
    }
}