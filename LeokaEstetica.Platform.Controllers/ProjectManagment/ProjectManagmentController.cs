using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.ProjectManagment;

/// <summary>
/// Контроллер управления проектами.
/// </summary>
[ApiController]
[Route("project-managment")]
[AuthFilter]
public class ProjectManagmentController : BaseController
{
   private readonly IProjectService _projectService;

   /// <summary>
   /// Конструктор.
   /// </summary>
   /// <param name="projectService">Сервис проектов пользователей.</param>
   public ProjectManagmentController(IProjectService projectService)
   {
      _projectService = projectService;
   }

   /// <summary>
   /// Метод получает список проектов пользователя.
   /// </summary>
   /// <returns>Список проектов пользователя.</returns>
   [HttpGet]
   [Route("user-projects")]
   [ProducesResponseType(200, Type = typeof(UserProjectResultOutput))]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task<UserProjectResultOutput> UserProjectsAsync()
   {
      var result = await _projectService.UserProjectsAsync(GetUserName(), false);

      return result;
   }
}