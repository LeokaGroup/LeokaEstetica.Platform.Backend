using Microsoft.AspNetCore.SignalR;

namespace LeokaEstetica.Platform.Notifications.Data;

/// <summary>
/// Класс хаба для обработки запросов SignalR.
/// </summary>
public class NotifyHub : Hub
{
    /// <summary>
    /// Метод отправляет уведомление всем подключенным клиентам.
    /// </summary>
    /// <param name="notifyText">Текст уведомления.</param>
    public async Task SendAsync(string notifyText)
    {
        await Clients.All.SendAsync("Receive", notifyText);
    }
}