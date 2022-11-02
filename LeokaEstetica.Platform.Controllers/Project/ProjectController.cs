using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Core.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Project;

/// <summary>
/// Контроллер работы с проектами.
/// </summary>
[AuthFilter]
[ApiController]
[Route("project")]
public class ProjectController : BaseController
{
    public ProjectController()
    {
    }

    /// <summary>
    /// TODO: Подумать, давать ли всем пользователям возможность просматривать каталог проектов или только тем, у кого есть подписка.
    /// </summary>
    [HttpGet]
    [Route("catalog")]
    public async Task CatalogProjectsAsync()
    {
        
    }
}