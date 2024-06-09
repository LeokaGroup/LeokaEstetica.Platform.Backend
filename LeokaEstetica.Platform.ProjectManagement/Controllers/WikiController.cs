using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
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
   private readonly ILogger<WikiController> _logger;
   private readonly IWikiTreeService _wikiTreeService;
   private readonly Lazy<IDiscordService> _discordService;

   /// <summary>
   /// Конструктор.
   /// <param name="logger">Логгер.</param>
   /// <param name="wikiTreeService">Сервис дерева.</param>
   /// <param name="wikiTreeService">Сервис уведомлений дискорда.</param>
   /// </summary>
   public WikiController(ILogger<WikiController> logger,
      IWikiTreeService wikiTreeService,
      Lazy<IDiscordService> discordService)
   {
      _logger = logger;
      _wikiTreeService = wikiTreeService;
      _discordService = discordService;
   }

   /// <summary>
   /// Метод получает дерево.
   /// </summary>
   /// <param name="projectId">Id проекта.</param>
   /// <returns>Дерево с вложенными элементами.</returns>
   [HttpGet]
   [Route("tree")]
   [ProducesResponseType(200, Type = typeof(IEnumerable<WikiTreeFolderItem>))]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task<IEnumerable<WikiTreeFolderItem>> GetTreeAsync([FromQuery] long projectId)
   {
      if (projectId <= 0)
      {
         var ex = new InvalidOperationException($"Ошибка получение дерева Wiki проекта. ProjectId: {projectId}");
         _logger.LogError(ex, ex.Message);
            
         await _discordService.Value.SendNotificationErrorAsync(ex);
            
         throw ex;
      }

      var result = await _wikiTreeService.GetTreeAsync(projectId);

      return result;
   }
}