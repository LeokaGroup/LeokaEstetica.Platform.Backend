using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.ProjectManagment.Controllers;

/// <summary>
/// Контроллер конфигураций модуля управления проектами.
/// </summary>
[AuthFilter]
[ApiController]
[Route("project-managment/config")]
public class ConfigController : BaseController
{
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly ILogger<ConfigController> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    /// <param name="logger">Логгер.</param>
    public ConfigController(IGlobalConfigRepository globalConfigRepository,
        ILogger<ConfigController> logger)
    {
        _globalConfigRepository = globalConfigRepository;
        _logger = logger;
    }

    /// <summary>
    /// Метод проверяет доступность модуля управления проектами.
    /// </summary>
    /// <returns>Признак активности модуля.</returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("is-available-project-managment")]
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<bool> IsAvailableProjectManagmentAsync()
    {
        _logger.LogInformation("Начали проверку доступности модуля управления проектами.");
        var result = await _globalConfigRepository.GetValueByKeyAsync<bool>(GlobalConfigKeys.ProjectManagment
                .PROJECT_MANAGMENT_MODE_ENABLED);
        _logger.LogInformation($"Закончили проверку доступности модуля управления проектами. Result: {result}");

        return result;
    }
}