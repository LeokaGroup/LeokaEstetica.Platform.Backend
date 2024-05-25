namespace LeokaEstetica.Platform.Redis.Abstractions.Client;

/// <summary>
/// Абстракция сервиса клиентов подключений.
/// </summary>
public interface IClientConnectionService
{
    /// <summary>
    /// Метод создает результат с клиентами, которым будут отправляться сообщения через сокеты.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Список клиентов с ConnectionId.</returns>
    Task<IEnumerable<string>> CreateClientsResultAsync(long? dialogId, long userId, string token);
}