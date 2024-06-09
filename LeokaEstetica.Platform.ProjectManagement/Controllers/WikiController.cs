using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.ProjectManagement.Controllers;

/// <summary>
/// Контроллер Wiki.
/// </summary>
[ApiController]
[Route("project-management-wiki")]
[AuthFilter]
public class WikiController : BaseController
{
   
}