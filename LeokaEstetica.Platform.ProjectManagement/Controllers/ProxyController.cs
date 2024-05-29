using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Models.Dto.Proxy.ProjectManagement;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.ProjectManagement.Controllers;

/// <summary>
/// Прокси-контроллер (выступает посредником между слоями во избежании ссылок).
/// </summary>
[ApiController]
[Route("project-management/proxy")]
[AuthFilter]
public class ProxyController : BaseController
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="configuration">Конфигурация приложения.</param>
    public ProxyController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Метод получает среду окружения из конфига модуля УП.
    /// </summary>
    /// <returns>Выходная модель настроек.</returns>
    [HttpGet]
    [Route("config-environment")]
    [ProducesResponseType(200, Type = typeof(ProxyConfigEnvironmentOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProxyConfigEnvironmentOutput> GetConfigEnvironmentAsync()
    {
        var result = new ProxyConfigEnvironmentOutput(_configuration["Environment"]!);

        return await Task.FromResult(result);
    }
    
    /// <summary>
    /// Метод получает конфиг модуля УП.
    /// </summary>
    /// <returns>Выходная модель настроек.</returns>
    [HttpGet]
    [Route("config-rabbitmq")]
    [ProducesResponseType(200, Type = typeof(ProxyConfigRabbitMqOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProxyConfigRabbitMqOutput> GetConfigAsync()
    {
        var result = new ProxyConfigRabbitMqOutput(_configuration["RabbitMq:HostName"],
            _configuration["RabbitMq:UserName"], _configuration["RabbitMq:Password"],
            _configuration["RabbitMq:VirtualHost"]);

        return await Task.FromResult(result);
    }
}