using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.ProjectManagement.Controllers;

/// <summary>
/// Контроллер работы с эпиками.
/// </summary>
[ApiController]
[Route("project-management/epics")]
[AuthFilter]
public class EpicController : BaseController
{
   private readonly ILogger<EpicController> _logger;
   private readonly Lazy<IDiscordService> _discordService;
   private readonly IEpicService _epicService;

   /// <summary>
   /// Конструктор.
   /// </summary>
   /// <param name="logger">Логгер.</param>
   /// <param name="discordService">Сервис уведомлений дискорда.</param>
   /// <param name="epicService">Сервис эпиков.</param>
   public EpicController(ILogger<EpicController> logger,
    Lazy<IDiscordService> discordService,
     IEpicService epicService)
   {
      _logger = logger;
      _discordService = discordService;
      _epicService = epicService;
   }

   /// <summary>
   /// Метод исключает задачи из эпика.
   /// </summary>
   /// <param name="includeExcludeEpicTaskInput">Входная модель.</param>
   [HttpPatch]
   [Route("epic-task")]
   [ProducesResponseType(200)]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task ExcludeEpicTasksAsync([FromBody] IncludeExcludeEpicSprintTaskInput includeExcludeEpicTaskInput)
   {
      if (includeExcludeEpicTaskInput.EpicSprintId <= 0
          || includeExcludeEpicTaskInput.ProjectTaskIds is null
          || includeExcludeEpicTaskInput.ProjectTaskIds.Any(string.IsNullOrWhiteSpace))
      {
         var ex = new InvalidOperationException(
            "Ошибка при исключении задач из эпика. " +
            $"EpicSprintId: {includeExcludeEpicTaskInput.EpicSprintId}. " +
            $"ProjectTaskIds: {JsonConvert.SerializeObject(includeExcludeEpicTaskInput.ProjectTaskIds)}.");
         _logger.LogError(ex, ex.Message);

         await _discordService.Value.SendNotificationErrorAsync(ex);

         throw ex;
      }

      await _epicService.ExcludeEpicTasksAsync(includeExcludeEpicTaskInput.EpicSprintId,
         includeExcludeEpicTaskInput.ProjectTaskIds);
   }
}