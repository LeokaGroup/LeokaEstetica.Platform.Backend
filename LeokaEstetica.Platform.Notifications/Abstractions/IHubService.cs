namespace LeokaEstetica.Platform.Notifications.Abstractions;

/// <summary>
/// TODO: Выпилить это все и абстракцию тоже и реализации в хабах.
/// TODO: Все это будет в одном хабе микросервиса коммуникаций.
/// Абстракция сервиса хаба.
/// </summary>
public interface IHubService
{
    /// <summary>
    /// Метод получает список диалогов.
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>
    /// <param name="projectId">Id проекта. Если не передан, то получает все диалоги пользователя.</param>
    /// <returns>Список диалогов.</returns>
    Task GetDialogsAsync(string account, string token, long? projectId = null);

    /// <summary>
    /// Метод получает диалог или создает новый и возвращает его.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>    
    /// <param name="dialogInput">Входная модель.</param>
    /// <returns>Данные диалога.</returns>
    Task GetDialogAsync(string account, string token, string dialogInput);

    /// <summary>
    /// Метод отправляет сообщение.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>    
    /// <param name="apiUrl">Api-url с фронта (текущий домен).</param>    
    /// <returns>Данные диалога с сообщениями. Обновляет диалог и сообщения диалога у всех участников диалога</returns>
    Task SendMessageAsync(string message, long dialogId, string account, string token, string apiUrl);

    /// <summary>
    /// Метод получает список диалогов для ЛК.
    /// </summary>
    /// <returns>Список диалогов.</returns>
    Task GetProfileDialogsAsync(string account, string token);
}