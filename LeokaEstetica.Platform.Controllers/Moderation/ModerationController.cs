using LeokaEstetica.Platform.Access.Abstractions.Moderation;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;
using LeokaEstetica.Platform.Moderation.Abstractions.Project;
using LeokaEstetica.Platform.Moderation.Models.Dto.Input;
using LeokaEstetica.Platform.Moderation.Models.Dto.Output;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Moderation;

/// <summary>
/// Контроллер модерации (отвечает за весь функционал модерации).
/// </summary>
[ApiController]
[Route("moderation")]
public class ModerationController : BaseController
{
    private readonly IAccessModerationService _accessModerationService;
    private readonly IProjectModerationService _projectModerationService;

    public ModerationController(IAccessModerationService accessModerationService, 
        IProjectModerationService projectModerationService)
    {
        _accessModerationService = accessModerationService;
        _projectModerationService = projectModerationService;
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
    public async Task<ModerationRoleOutput> CheckUserRoleModerationAsync(
        [FromBody] ModerationRoleInput moderationRoleInput)
    {
        var result = await _accessModerationService.CheckUserRoleModerationAsync(moderationRoleInput.Email);

        return result;
    }

    /// <summary>
    /// Метод получает список проектов для модерации.
    /// </summary>
    /// <returns>Список проектов.</returns>
    [HttpGet]
    [Route("projects")]
    [ProducesResponseType(200, Type = typeof(ProjectsModerationResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectsModerationResult> ProjectsModerationAsync()
    {
        var result = await _projectModerationService.ProjectsModerationAsync();

        return result;
    }
}