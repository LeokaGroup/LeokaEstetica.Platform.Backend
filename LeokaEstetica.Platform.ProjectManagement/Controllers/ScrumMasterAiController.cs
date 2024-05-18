using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.ProjectManagement.Controllers;

/// <summary>
/// Контроллер нейросети ScrumMasterAi.
/// </summary>
[ApiController]
[Route("project-management/scrum-master-ai")]
[AuthFilter]
public class ScrumMasterAiController : BaseController
{
    private readonly ILogger<ScrumMasterAiController> _logger;
    private readonly Lazy<IDiscordService> _discordService;
    
    /// <summary>
    /// Конструктор.
    /// <param name="logger">Логгер.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// </summary>
    public ScrumMasterAiController(ILogger<ScrumMasterAiController> logger,
     Lazy<IDiscordService> discordService)
    {
        _logger = logger;
        _discordService = discordService;
    }

    /// <summary>
    /// TODO: Добавить ограничение на роль пользователя. Только некоторым можно учить ее, проверять на роль.
    /// Метод обучает нейросеть из датасета в формате csv.
    /// </summary>
    /// <param name="file">Датасет в csv.</param>
    [HttpPost]
    [Route("education")]
    public async Task EducationFromCsvDatasetAsync([FromForm] IFormFile file)
    {
        if (file.Length == 0)
        {
            var ex = new InvalidOperationException(
                "Не передан файл для обучения нейросети. Требуемый формат файла: .CSV");
            _logger.LogError(ex ,ex.Message);

            await _discordService.Value.SendNotificationErrorAsync(ex);
        }
    }
}