using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.ProjectManagement.ScrumMasterAI.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.ProjectManagement.Controllers;

/// <summary>
/// Контроллер нейросети ScrumMasterAi.
/// </summary>
[ApiController]
[Route("project-management/scrum-master-ai")]
// [AuthFilter]
public class ScrumMasterAiController : BaseController
{
    private readonly ILogger<ScrumMasterAiController> _logger;
    private readonly Lazy<IDiscordService> _discordService;
    private readonly IScrumMasterAiService _scrumMasterAiService;
    
    /// <summary>
    /// Конструктор.
    /// <param name="logger">Логгер.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="scrumMasterAiService">Сервис нейросети.</param>
    /// </summary>
    public ScrumMasterAiController(ILogger<ScrumMasterAiController> logger,
     Lazy<IDiscordService> discordService,
      IScrumMasterAiService scrumMasterAiService)
    {
        _logger = logger;
        _discordService = discordService;
        _scrumMasterAiService = scrumMasterAiService;
    }

    /// <summary>
    /// TODO: Добавить ограничение на роль пользователя. Только некоторым можно учить ее, проверять на роль.
    /// Метод обучает нейросеть из датасета в формате csv.
    /// </summary>
    [HttpPost]
    [Route("education-csv")]
    public async Task EducationFromCsvDatasetAsync()
    {
        await _scrumMasterAiService.EducationFromCsvDatasetAsync();
    }
    
    /// <summary>
    /// TODO: Добавить ограничение на роль пользователя. Только некоторым можно учить ее, проверять на роль.
    /// Метод обучает нейросеть из датасета в формате ONNX.
    /// ONNX - Этот формат является универсальным для машинного обучения
    /// (можно из Python например импортировать уже обученную модель, если она у формате .ONNX).
    /// </summary>
    /// <param name="file">Датасет в ONNX.</param>
    [HttpPost]
    [Route("education-onnx")]
    public async Task EducationFromOnnxDatasetAsync([FromForm] IFormFile file)
    {
        if (file.Length == 0)
        {
            var ex = new InvalidOperationException(
                "Не передан файл для обучения нейросети. Требуемый формат файла: .ONNX");
            _logger.LogError(ex ,ex.Message);

            await _discordService.Value.SendNotificationErrorAsync(ex);
        }

        throw new NotImplementedException("Обучение из формата .ONNX пока не реализовано.");
    }
}