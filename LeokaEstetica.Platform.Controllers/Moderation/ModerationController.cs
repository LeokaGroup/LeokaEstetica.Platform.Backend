using LeokaEstetica.Platform.Access.Abstractions.Moderation;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Moderation.Models.Dto.Output;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Moderation;

/// <summary>
/// Контроллер модерации (отвечает за весь функционал модерации).
/// </summary>
[AuthFilter]
[ApiController]
[Route("moderation")]
public class ModerationController : BaseController
{
    private readonly IAccessModerationService _accessModerationService;
    
    public ModerationController(IAccessModerationService accessModerationService)
    {
        _accessModerationService = accessModerationService;
    }

    /// <summary>
    /// Метод проверяет, имеет ли пользователь роль, которая дает доступ к модерации.
    /// </summary>
    /// <returns>Данные выходной модели.</returns>
    [HttpPost]
    [Route("check")]
    [ProducesResponseType(200, Type = typeof(ModerationRoleOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ModerationRoleOutput> CheckUserRoleModerationAsync()
    {
        var result = await _accessModerationService.CheckUserRoleModerationAsync(GetUserName());

        return result;
    }
}