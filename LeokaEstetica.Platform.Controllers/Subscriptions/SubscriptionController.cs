using AutoMapper;
using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Core.Filters;
using LeokaEstetica.Platform.Models.Dto.Output.Subscription;
using LeokaEstetica.Platform.Services.Abstractions.Subscription;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Subscriptions;

/// <summary>
/// Контроллер работы с подписками.
/// </summary>
[AuthFilter]
[ApiController]
[Route("subscriptions")]
public class SubscriptionController : BaseController
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IMapper _mapper;
    
    /// <inheritdoc />
    public SubscriptionController(ISubscriptionService subscriptionService, 
        IMapper mapper)
    {
        _subscriptionService = subscriptionService;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод получает список подписок.
    /// </summary>
    /// <returns>Список подписок.</returns>
    [HttpGet]
    [Route("")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<SubscriptionOutput>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<IEnumerable<SubscriptionOutput>> GetSubscriptionsAsync()
    {
        // Получаем список подписок.
        var subscriptions = await _subscriptionService.GetSubscriptionsAsync();
        var mapsubscriptions = _mapper.Map<List<SubscriptionOutput>>(subscriptions);
        
        // Проставляем выделенную подписку пользователю, если он оформлял ее.
        var result = await _subscriptionService.FillSubscriptionsAsync(GetUserName(), mapsubscriptions);

        return result;
    }
}