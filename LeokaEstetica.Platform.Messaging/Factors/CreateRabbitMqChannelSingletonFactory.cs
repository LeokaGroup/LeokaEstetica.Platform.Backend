using RabbitMQ.Client;

namespace LeokaEstetica.Platform.Messaging.Factors;

/// <summary>
/// Класс факторки, которая создает канал подключения как синглтон.
/// </summary>
public static class CreateRabbitMqChannelSingletonFactory
{
    private static IModel _model;

    /// <summary>
    /// Метод создает модель подключения канала к кролику.
    /// </summary>
    /// <param name="connection">Подключение к кролику.</param>
    /// <returns>Объект с данными модели подключения канала к кролику.</returns>
    public static IModel CreateRabbitMqChannel(IConnection connection)
    {
        if (_model == null)
        {
            _model = connection.CreateModel();
        }

        return _model;
    }
}