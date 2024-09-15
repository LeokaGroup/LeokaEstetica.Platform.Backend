using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.ProjectManagement.Validators;
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
   /// <param name="excludeEpicTaskInput">Входная модель.</param>
   [HttpPatch]
   [Route("epic")]
   [ProducesResponseType(200)]
   [ProducesResponseType(400)]
   [ProducesResponseType(403)]
   [ProducesResponseType(500)]
   [ProducesResponseType(404)]
   public async Task ExcludeEpicTasksAsync([FromBody] ExcludeEpicTaskInput excludeEpicTaskInput)
   {
      var validator = await new ExcludeEpicTaskValidator().ValidateAsync(excludeEpicTaskInput);

      if (validator.Errors.Count > 0)
      {
         var exceptions = new List<InvalidOperationException>();

         foreach (var err in validator.Errors)
         {
            exceptions.Add(new InvalidOperationException(err.ErrorMessage));
         }

         var ex = new AggregateException(
            "Ошибка при исключении задач из эпика. " +
            $"EpicId: {excludeEpicTaskInput.EpicId}. " +
            $"ProjectTaskIds: {JsonConvert.SerializeObject(excludeEpicTaskInput.ProjectTaskIds)}",
            exceptions);
         _logger.LogError(ex, ex.Message);
            
         await _discordService.Value.SendNotificationErrorAsync(ex);
            
         throw ex;
      }

      await _epicService.ExcludeEpicTasksAsync(excludeEpicTaskInput.EpicId, excludeEpicTaskInput.ProjectTaskIds);
   }
}