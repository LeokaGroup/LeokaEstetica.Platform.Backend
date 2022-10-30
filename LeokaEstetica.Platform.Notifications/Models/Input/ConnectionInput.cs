namespace LeokaEstetica.Platform.Notifications.Models.Input;

/// <summary>
/// Класс входной модели подключения для SignalR.
/// </summary>
public class ConnectionInput
{
    /// <summary>
    /// Id подключения.
    /// </summary>
    public string ConnectionId { get; set; }

    /// <summary>
    /// Код подключения.
    /// </summary>
    public string UserCode { get; set; }
}