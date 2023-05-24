namespace LeokaEstetica.Platform.Base.Abstractions.Messaging.EventBus;

public interface IDynamicIntegrationEventHandler : IIntegrationEventHandler
{
    Task Handle(dynamic eventData);
}