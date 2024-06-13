using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;
using LeokaEstetica.Platform.ProjectManagement.Validators;
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

   /// <summary>
   /// Метод получает папку (и ее структуру - вложенные папки и страницы).
   /// </summary>
   /// <param name="projectId">Id проекта.</param>
   /// <param name="folderId">Id папки.</param>
   /// <returns>Структура папки.</returns>
   [HttpGet]
   [Route("tree-item-folder")]
   [ProducesResponseType(200, Type = typeof(IEnumerable<WikiTreeFolderItem>))]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task<IEnumerable<WikiTreeFolderItem>> GetTreeItemFolderAsync([FromQuery] long projectId,
      [FromQuery] long folderId)
   {
      var validator = await new GetTreeItemFolderValidator().ValidateAsync((projectId, folderId));

      if (validator.Errors.Any())
      {
         var exceptions = new List<InvalidOperationException>();

         foreach (var err in validator.Errors)
         {
            exceptions.Add(new InvalidOperationException(err.ErrorMessage));
         }
            
         var ex = new AggregateException("Ошибка получения структуры папки Wiki проекта.", exceptions);
         _logger.LogError(ex, ex.Message);
            
         await _discordService.Value.SendNotificationErrorAsync(ex);
            
         throw ex;
      }
      
      var result = await _wikiTreeService.GetTreeItemFolderAsync(projectId, folderId);

      return result;
   }
   
   /// <summary>
   /// Метод получает содержимое страницы.
   /// </summary>
   /// <param name="pageId">Id страницы.</param>
   /// <returns>Содержимое страницы.</returns>
   [HttpGet]
   [Route("tree-item-page")]
   [ProducesResponseType(200, Type = typeof(WikiTreePageItem))]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task<WikiTreePageItem> GetTreeItemPageAsync([FromQuery] long pageId)
   {
      if (pageId <= 0)
      {
         var ex = new InvalidOperationException("Id страницы Wiki проекта не передан или невалиден. " +
                                                $"PageId: {pageId}");
         _logger.LogError(ex, ex.Message);
            
         await _discordService.Value.SendNotificationErrorAsync(ex);
            
         throw ex;
      }

      var result = await _wikiTreeService.GetTreeItemPageAsync(pageId);

      return result;
   }
   
   /// <summary>
   /// Метод изменяет название папки.
   /// </summary>
   /// <param name="updateFolderNameInput">Входная модель.</param>
   [HttpPatch]
   [Route("tree-item-folder")]
   [ProducesResponseType(200, Type = typeof(WikiTreeFolderItem))]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task UpdateFolderNameAsync([FromBody] UpdateFolderNameInput updateFolderNameInput)
   {
      var validator = await new ChangeFolderNameValidator().ValidateAsync(updateFolderNameInput);

      if (validator.Errors.Any())
      {
         var exceptions = new List<InvalidOperationException>();

         foreach (var err in validator.Errors)
         {
            exceptions.Add(new InvalidOperationException(err.ErrorMessage));
         }
            
         var ex = new AggregateException("Ошибка изменения названия папки Wiki проекта.", exceptions);
         _logger.LogError(ex, ex.Message);
            
         await _discordService.Value.SendNotificationErrorAsync(ex);
            
         throw ex;
      }

      await _wikiTreeService.UpdateFolderNameAsync(updateFolderNameInput.FolderName, updateFolderNameInput.FolderId);
   }
}