using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Models.Dto.Input.Config;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.ProjectManagment.Validators;
using LeokaEstetica.Platform.Services.Abstractions.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.ProjectManagment.Controllers;

/// <summary>
/// Контроллер конфигураций модуля управления проектами.
/// </summary>
[AuthFilter]
[ApiController]
[Route("project-management/config")]
public class ConfigController : BaseController
{
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly ILogger<ConfigController> _logger;
    private readonly IProjectSettingsConfigService _projectSettingsConfigService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="projectSettingsConfigService">Сервис настроек проектов.</param>
    public ConfigController(IGlobalConfigRepository globalConfigRepository,
        ILogger<ConfigController> logger,
        IProjectSettingsConfigService projectSettingsConfigService)
    {
        _globalConfigRepository = globalConfigRepository;
        _logger = logger;
        _projectSettingsConfigService = projectSettingsConfigService;
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
                .PROJECT_MANAGEMENT_MODE_ENABLED);
        _logger.LogInformation($"Закончили проверку доступности модуля управления проектами. Result: {result}");

        return result;
    }

    /// <summary>
    /// Метод фиксирует выбранные пользователем настройки рабочего пространства проекта.
    /// </summary>
    /// <param name="configSpaceSettingInput">Входная модель.</param>
    /// <returns>Выходная модель.</returns>
    [HttpPost]
    [Route("space-settings")]
    [ProducesResponseType(200, Type = typeof(ConfigSpaceSettingOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ConfigSpaceSettingOutput> CommitSpaceSettingsAsync(
        [FromBody] ConfigSpaceSettingInput configSpaceSettingInput)
    {
        var validator = await new CommitProjectManagementSpaceSettingValidator()
            .ValidateAsync(configSpaceSettingInput);
        
        if (validator.Errors.Any())
        {
            var exceptions = new List<InvalidOperationException>();

            foreach (var err in validator.Errors)
            {
                exceptions.Add(new InvalidOperationException(err.ErrorMessage));
            }

            var ex = new AggregateException(
                "Ошибка фиксации настроек рабочего пространств." +
                $" ConfigSpaceSettingInput: {JsonConvert.SerializeObject(configSpaceSettingInput)}",
                exceptions);
            _logger.LogError(ex, ex.Message);
            
            throw ex;
        }

        var result = await _projectSettingsConfigService.CommitSpaceSettingsAsync(configSpaceSettingInput.Strategy,
            configSpaceSettingInput.TemplateId, configSpaceSettingInput.ProjectId, GetUserName(),
            configSpaceSettingInput.ProjectManagementName, configSpaceSettingInput.ProjectManagementNamePrefix);

        return result;
    }

    /// <summary>
    /// Метод получает Id проекта, который был ранее выбран пользователем для перехода к управлению проектом.
    /// Необходимо для построения ссылки в рабочее пространство проекта.
    /// </summary>
    /// <returns>Выходная модель.</returns>
    [HttpGet]
    [Route("build-project-space")]
    [ProducesResponseType(200, Type = typeof(ConfigSpaceSettingOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ConfigSpaceSettingOutput> GetBuildProjectSpaceSettingsAsync()
    {
        var result = await _projectSettingsConfigService.GetBuildProjectSpaceSettingsAsync(GetUserName());

        return result;
    }
}