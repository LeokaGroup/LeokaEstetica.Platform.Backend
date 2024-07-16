using LeokaEstetica.Platform.Access.Models.Output;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Access;

/// <summary>
/// Контроллер проверки доступов к разным модулям, компонентам платформы.
/// </summary>
[AuthFilter]
[ApiController]
[Route("access")]
public class AccessController : BaseController
{
    public AccessController()
    {
    }

    /// <summary>
    /// Метод проверяет доступ к модулям, компонентам модуля УП.
    /// Бесплатные компоненты не проверяются.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="accessModule">Тип модуля, компонента.</param>
    /// <returns></returns>
    [HttpGet]
    [Route("access-module")]
    [ProducesResponseType(200, Type = typeof(AccessProjectManagementOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<AccessProjectManagementOutput> CheckAccessProjectManagementModuleOrComponentAsync([FromQuery] long projectId,
        [FromQuery] AccessModuleTypeEnum accessModule)
    {
        
    }
}