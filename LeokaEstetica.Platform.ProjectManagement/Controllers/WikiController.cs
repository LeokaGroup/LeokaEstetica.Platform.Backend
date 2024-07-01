using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;
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
   private readonly Lazy<IWikiTreeRepository> _wikiTreeRepository;

   /// <summary>
   /// Конструктор.
   /// <param name="logger">Логгер.</param>
   /// <param name="wikiTreeService">Сервис дерева.</param>
   /// <param name="wikiTreeService">Сервис уведомлений дискорда.</param>
   /// <param name="wikiTreeRepository">Репозиторий Wiki.</param>
   /// </summary>
   public WikiController(ILogger<WikiController> logger,
      IWikiTreeService wikiTreeService,
      Lazy<IDiscordService> discordService,
       Lazy<IWikiTreeRepository> wikiTreeRepository)
   {
      _logger = logger;
      _wikiTreeService = wikiTreeService;
      _discordService = discordService;
      _wikiTreeRepository = wikiTreeRepository;
   }

   /// <summary>
   /// Метод получает дерево.
   /// </summary>
   /// <param name="projectId">Id проекта.</param>
   /// <returns>Дерево с вложенными элементами.</returns>
   [HttpGet]
   [Route("tree")]
   [ProducesResponseType(200, Type = typeof(IEnumerable<WikiTreeItem>))]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task<IEnumerable<WikiTreeItem>> GetTreeAsync([FromQuery] long projectId)
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
   [ProducesResponseType(200, Type = typeof(IEnumerable<WikiTreeItem>))]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task<IEnumerable<WikiTreeItem>> GetTreeItemFolderAsync([FromQuery] long projectId,
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
   [ProducesResponseType(200, Type = typeof(WikiTreeItem))]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task<WikiTreeItem> GetTreeItemPageAsync([FromQuery] long pageId)
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
   [ProducesResponseType(200)]
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
   
   /// <summary>
   /// Метод изменяет название страницы папки.
   /// </summary>
   /// <param name="updateFolderPageNameInput">Входная модель.</param>
   [HttpPatch]
   [Route("tree-item-folder-page-name")]
   [ProducesResponseType(200)]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task UpdateFolderPageNameAsync(
      [FromBody] UpdateFolderPageNameInput updateFolderPageNameInput)
   {
      var validator = await new ChangePageFolderNameValidator().ValidateAsync(updateFolderPageNameInput);

      if (validator.Errors.Any())
      {
         var exceptions = new List<InvalidOperationException>();

         foreach (var err in validator.Errors)
         {
            exceptions.Add(new InvalidOperationException(err.ErrorMessage));
         }
            
         var ex = new AggregateException("Ошибка изменения названия страницы папки Wiki проекта.", exceptions);
         _logger.LogError(ex, ex.Message);
            
         await _discordService.Value.SendNotificationErrorAsync(ex);
            
         throw ex;
      }

      await _wikiTreeService.UpdateFolderPageNameAsync(updateFolderPageNameInput.PageName,
         updateFolderPageNameInput.PageId);
   }
   
   /// <summary>
   /// Метод изменяет описание страницы страницы.
   /// </summary>
   /// <param name="updateFolderPageDescriptionInput">Входная модель.</param>
   [HttpPatch]
   [Route("tree-item-folder-page-description")]
   [ProducesResponseType(200)]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task UpdateFolderPageDescriptionAsync(
      [FromBody] UpdateFolderPageDescriptionInput updateFolderPageDescriptionInput)
   {
      var validator = await new ChangePageFolderDescriptionValidator().ValidateAsync(updateFolderPageDescriptionInput);

      if (validator.Errors.Any())
      {
         var exceptions = new List<InvalidOperationException>();

         foreach (var err in validator.Errors)
         {
            exceptions.Add(new InvalidOperationException(err.ErrorMessage));
         }
            
         var ex = new AggregateException("Ошибка изменения описания страницы папки Wiki проекта.", exceptions);
         _logger.LogError(ex, ex.Message);
            
         await _discordService.Value.SendNotificationErrorAsync(ex);
            
         throw ex;
      }

      await _wikiTreeService.UpdateFolderPageDescriptionAsync(updateFolderPageDescriptionInput.PageDescription,
         updateFolderPageDescriptionInput.PageId);
   }

   /// <summary>
   /// Метод получает элементы контекстного меню.
   /// </summary>
   /// <param name="projectId">Id проекта, если передан.</param>
   /// <param name="pageId">Id страницы, если передан.</param>
   /// <param name="isParentFolder">Признак создания вне родителя.</param>
   /// <returns>Элементы контекстного меню.</returns>
   [HttpGet]
   [Route("context-menu")]
   [ProducesResponseType(200, Type = typeof(IEnumerable<WikiContextMenuOutput>))]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task<IEnumerable<WikiContextMenuOutput>> GetContextMenuAsync([FromQuery] long? projectId,
      [FromQuery] long? pageId, [FromQuery] bool isParentFolder = false)
   {
      if (!projectId.HasValue && !pageId.HasValue)
      {
         var ex = new InvalidOperationException("Ошибка получения контекстного меню Wiki проекта. " +
                                                "ProjectId и PageId не были переданы.");
         _logger.LogError(ex, ex.Message);
            
         await _discordService.Value.SendNotificationErrorAsync(ex);
            
         throw ex;
      }
      
      var result = await _wikiTreeRepository.Value.GetContextMenuAsync(projectId, pageId, isParentFolder);

      return result;
   }

   /// <summary>
   /// Метод создает папку.
   /// </summary>
   /// <param name="createWikiFolderInput">Входная модель.</param>
   [HttpPost]
   [Route("tree-item-folder-page")]
   [ProducesResponseType(200)]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task CreateFolderAsync([FromBody] CreateWikiFolderInput createWikiFolderInput)
   {
      var validator = await new CreateFolderValidator().ValidateAsync(createWikiFolderInput);

      if (validator.Errors.Any())
      {
         var exceptions = new List<InvalidOperationException>();

         foreach (var err in validator.Errors)
         {
            exceptions.Add(new InvalidOperationException(err.ErrorMessage));
         }
            
         var ex = new AggregateException("Ошибка создания папки Wiki проекта.", exceptions);
         _logger.LogError(ex, ex.Message);
            
         await _discordService.Value.SendNotificationErrorAsync(ex);
            
         throw ex;
      }

      await _wikiTreeService.CreateFolderAsync(createWikiFolderInput.ParentId, createWikiFolderInput.FolderName,
         GetUserName(), createWikiFolderInput.WikiTreeId);
   }
   
   /// <summary>
   /// Метод создает страницу.
   /// </summary>
   /// <param name="createWikiPageInput">Входная модель.</param>
   [HttpPost]
   [Route("tree-item-page")]
   [ProducesResponseType(200)]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task CreatePageAsync([FromBody] CreateWikiPageInput createWikiPageInput)
   {
      var validator = await new CreatePageValidator().ValidateAsync(createWikiPageInput);

      if (validator.Errors.Any())
      {
         var exceptions = new List<InvalidOperationException>();

         foreach (var err in validator.Errors)
         {
            exceptions.Add(new InvalidOperationException(err.ErrorMessage));
         }
            
         var ex = new AggregateException("Ошибка создания страницы Wiki проекта.", exceptions);
         _logger.LogError(ex, ex.Message);
            
         await _discordService.Value.SendNotificationErrorAsync(ex);
            
         throw ex;
      }

      await _wikiTreeService.CreatePageAsync(createWikiPageInput.ParentId, createWikiPageInput.PageName,
         GetUserName(), createWikiPageInput.WikiTreeId);
   }

   /// <summary>
   /// Метод удаляет папку.
   /// Удаляет все дочерние папки и страницы у этой папки.
   /// </summary>
   /// <param name="folderId">Id папки.</param>
   /// <param name="isApprove">Признак согласия пользователя на удаление дочерних элементов.</param>
   /// <returns>Выходная модель.</returns>
   [HttpDelete]
   [Route("tree-item-folder")]
   [ProducesResponseType(200, Type = typeof(RemoveFolderResponseOutput))]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task<RemoveFolderResponseOutput> RemoveFolderAsync([FromQuery] long folderId,
      [FromQuery] bool isApprove)
   {
      var result = await _wikiTreeService.RemoveFolderAsync(folderId, isApprove);

      return result;
   }
   
   /// <summary>
   /// Метод удаляет страницу.
   /// </summary>
   /// <param name="pageId">Id страницы.</param>
   [HttpDelete]
   [Route("tree-item-page")]
   [ProducesResponseType(200)]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task RemovePageAsync([FromQuery] long pageId)
   {
      await _wikiTreeService.RemovePageAsync(pageId);
   }
}