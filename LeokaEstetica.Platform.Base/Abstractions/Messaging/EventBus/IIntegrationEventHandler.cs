namespace LeokaEstetica.Platform.Base.Abstractions.Messaging.EventBus;

public interface IIntegrationEventHandler
{
    void Stop();
}

public interface IIntegrationEventHandler<in T> : IIntegrationEventHandler
    where T : IIntegrationEvent
{
    Task Handle(T @event);
}