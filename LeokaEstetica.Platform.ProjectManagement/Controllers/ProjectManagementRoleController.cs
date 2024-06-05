using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.ProjectManagement.Controllers;

/// <summary>
/// Контроллер ролей управления проектами.
/// </summary>
[ApiController]
[Route("project-management-role")]
[AuthFilter]
public class ProjectManagementRoleController : BaseController
{
    
}