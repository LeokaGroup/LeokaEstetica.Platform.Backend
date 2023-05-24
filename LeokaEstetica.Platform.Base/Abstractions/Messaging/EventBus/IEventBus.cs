namespace LeokaEstetica.Platform.Base.Abstractions.Messaging.EventBus;

public interface IEventBus
{
    /// <summary>
    /// Creates exchange, queue and consumer for events
    /// </summary>
    void StartEventBus();

    /// <summary>
    /// Closes exchange and stop events consuming
    /// </summary>
    void StopEventBus();

    /// <summary>
    /// Publishes an event (async).
    /// </summary>
    /// <param name="event">Event to publish</param>
    // Task Publish(IIntegrationEvent @event, bool isLogEvent = true);
    void Publish(IIntegrationEvent @event);

    Task Publish(IIntegrationEvent @event, string routingKey, bool isLogEvent = true);

    /// <summary>
    /// Publishes an event (sync).
    /// </summary>
    /// <param name="event">Event to publish</param>
    void PublishSync(IIntegrationEvent @event, bool isLogEvent = true);

    /// <summary>
    /// Subsctibes a handler for specified event
    /// </summary>
    /// <typeparam name="E">Type of event for subscribing</typeparam>
    /// <typeparam name="EH">Type of handler for subscribing</typeparam>
    void Subscribe<E, EH>()
        where E : IIntegrationEvent
        where EH : IIntegrationEventHandler<E>;

    /// <summary>
    /// Unsubsctibes a handler from specified event
    /// </summary>
    /// <typeparam name="E">Type of event for usubscribing</typeparam>
    /// <typeparam name="EH">Type of handler for usubscribing</typeparam>
    void Unsubscribe<E, EH>()
        where E : IIntegrationEvent
        where EH : IIntegrationEventHandler<E>;

    void SubscribeDynamic<EH>(string eventName)
        where EH : IDynamicIntegrationEventHandler;

    void UnsubscribeDynamic<EH>(string eventName)
        where EH : IDynamicIntegrationEventHandler;
}