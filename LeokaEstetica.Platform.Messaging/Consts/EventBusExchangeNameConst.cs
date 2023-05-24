namespace LeokaEstetica.Platform.Messaging.Consts;

internal static class EventBusExchangeNameConst
{
    public const int CountTrySendSendMessages = 3;
    public const string QueueName = "Messaging{0}-{1}";
    public const string MessageExchangeName = "Messaging";
    public const string ConsumerClient = ".Messaging.Consumer";
    public const string ProducerClient = ".Messaging.Producer";
    public const string RoutingKey = "messaging.{0}.{1}";
    public const string RoutingAllKey = "messaging.all";
    public const string ConsumerConnectionString = "Rabbit.Messaging.Consumer";
    public const string ProducerConnectionString = "Rabbit.Messaging.Producer";
    public const string ProducerParserConnectionString = "Rabbit.Messaging.Parser.Producer";
    public const string EventBusConnectionString = "Rabbit.EventBus";
    public const string EventBusQueueName = "{0}_Events";

    /// <summary>
    /// The time after which the queue is deleted, after disabling the Consumer.
    /// </summary>
    public static readonly TimeSpan QueueExpires = TimeSpan.FromMinutes(5);

    /// <summary>
    /// The lifetime of the queues for storing events.
    /// </summary>
    public static readonly TimeSpan EventQueueExpires = TimeSpan.FromHours(12);

    public const string DataProcessorExchangeName = "DataProcessorExchange";
    public const string DelayDataProcessorExchangeName = "DelayedDataProcessorExchange";
    public const string EventBusExchangeName = "EventBusExchange";
}