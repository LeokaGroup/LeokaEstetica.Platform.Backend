using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Communications.Controllers;

/// <summary>
/// Контроллер групп абстрактной области чата.
/// </summary>
[ApiController]
[Route("abstract-group")]
[AuthFilter]
public class AbstractGroupController : BaseController
{
    private readonly ILogger<AbstractGroupController> _logger;
    private readonly Lazy<IDiscordService> _discordService;
    private readonly IAbstractGroupService _abstractGroupService;
    
    /// <summary>
    /// Конструктор.
    /// <param name="logger">Логгер.</param>
    /// <param name="discordService">Логгер.</param>
    /// <param name="abstractGroupService">Сервис групп абстрактной области чата.</param>
    /// </summary>
    public AbstractGroupController(ILogger<AbstractGroupController> logger,
        Lazy<IDiscordService> discordService, 
        IAbstractGroupService abstractGroupService)
    {
        _logger = logger;
        _discordService = discordService;
        _abstractGroupService = abstractGroupService;
    }

    /// <summary>
    /// TODO: Изменить на сокеты.
    /// Метод получает объекты группы абстрактной области.
    /// </summary>
    /// <param name="abstractScopeId">Id абстрактной области.</param>
    /// <param name="abstractScopeType">Тип абстрактной области.</param>
    /// <returns>Объекты группы абстрактной области.</returns>
    [HttpGet]
    [Route("objects")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<AbstractGroupOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<AbstractGroupResult> GetAbstractGroupObjectsAsync([FromQuery] long abstractScopeId,
        [FromQuery] AbstractScopeTypeEnum abstractScopeType)
    {
        if (abstractScopeId <= 0)
        {
            var ex = new InvalidOperationException("Ошибка валидации Id группы абстрактной области чата. " +
                                                   $"AbstractScopeId: {abstractScopeId}.");

            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            _logger.LogError(ex, ex.Message);
        }

        if (abstractScopeType == AbstractScopeTypeEnum.Undefined)
        {
            var ex = new InvalidOperationException("Ошибка валидации типа абстрактной области чата. " +
                                                   $"AbstractScopeType: {abstractScopeType}.");

            await _discordService.Value.SendNotificationErrorAsync(ex);
            
            _logger.LogError(ex, ex.Message);
        }

        var result = await _abstractGroupService.GetAbstractGroupObjectsAsync(abstractScopeId, abstractScopeType,
            GetUserName());

        return result;
    }
}