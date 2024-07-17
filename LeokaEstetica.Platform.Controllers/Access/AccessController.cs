using LeokaEstetica.Platform.Access.Abstractions.ProjectManagement;
using LeokaEstetica.Platform.Access.Models.Output;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Controllers.Validators.Access;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Controllers.Access;

/// <summary>
/// Контроллер проверки доступов к разным модулям, компонентам платформы.
/// </summary>
[AuthFilter]
[ApiController]
[Route("access")]
public class AccessController : BaseController
{
    private readonly ILogger<AccessController> _logger;
    private readonly Lazy<IDiscordService> _discordService;
    private readonly IAccessModuleService _accessModuleService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="accessModuleService">Сервис проверки доступов к модулям или компонентам.</param>
    public AccessController(ILogger<AccessController> logger,
     Lazy<IDiscordService> discordService,
     IAccessModuleService accessModuleService)
    {
        _logger = logger;
        _discordService = discordService;
        _accessModuleService = accessModuleService;
    }

    /// <summary>
    /// Метод проверяет доступ к модулям, компонентам модуля УП.
    /// Бесплатные компоненты не проверяются.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="accessModule">Тип модуля.</param>
    /// <param name="accessModuleComponentType">Тип компонента, к которому проверяется доступ.</param>
    /// <returns>Модель с результатами проверки доступа, к которому проверяется доступ.</returns>
    [HttpGet]
    [Route("access-module")]
    [ProducesResponseType(200, Type = typeof(AccessProjectManagementOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<AccessProjectManagementOutput> CheckAccessProjectManagementModuleOrComponentAsync(
        [FromQuery] long projectId, [FromQuery] AccessModuleTypeEnum accessModule,
        [FromQuery] AccessModuleComponentTypeEnum accessModuleComponentType)
    {
        var validator = await new CheckAccessProjectManagementModuleOrComponentValidator().ValidateAsync(
            (projectId, accessModule, accessModuleComponentType));
        
        if (validator.Errors.Count > 0)
        {
            var ex = new InvalidOperationException(
                "Параметры для проверки доступа к модулям или компонентам не прошли валидацию. ");
                
            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            _logger.LogError(ex, ex.Message);

            throw ex;
        }

        var result = await _accessModuleService.CheckAccessProjectManagementModuleOrComponentAsync(projectId,
            accessModule, accessModuleComponentType);

        return result;
    }
}