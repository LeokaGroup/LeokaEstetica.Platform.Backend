using LeokaEstetica.Platform.Base.Models.Dto;

namespace LeokaEstetica.Platform.Base.Abstractions.Messaging.EventBus;

public interface IEventBusHandlersManager
{
    bool IsEmpty { get; }

    void AddSubscription<E, EH>()
        where E : IIntegrationEvent
        where EH : IIntegrationEventHandler<E>;

    void RemoveSubscription<E, EH>()
        where E : IIntegrationEvent
        where EH : IIntegrationEventHandler<E>;

    void AddDynamicSubscription<T>(string eventName)
        where T : IDynamicIntegrationEventHandler;

    void RemoveDynamicSubscription<T>(string eventName)
        where T : IDynamicIntegrationEventHandler;

    bool HasSubscriptionsForEvent<T>() where T : IIntegrationEvent;

    bool HasSubscriptionsForEvent(string eventName);

    Type GetEventTypeByName(string eventName);

    void Clear();

    IEnumerable<HandlerInfo> GetHandlersForEvent<T>() where T : IIntegrationEvent;

    IEnumerable<HandlerInfo> GetHandlersForEvent(string eventName);

    string GetEventKey<T>();
}