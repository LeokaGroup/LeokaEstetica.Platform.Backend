using LeokaEstetica.Platform.Models.Dto.Communications.Output;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Communications.Abstractions;

/// <summary>
/// Абстракция сервиса сообщений диалога группы объектов абстрактной области чата.
/// </summary>
public interface IAbstractGroupDialogMessagesService
{
    /// <summary>
    /// Метод получает список сообщений диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список сообщений диалога.</returns>
    Task<IEnumerable<GroupObjectDialogMessageOutput>> GetObjectDialogMessagesAsync(long dialogId, string account);

    /// <summary>
    /// Метод отправляет сообщение в очередь RabbitMQ.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="userCode">Код пользователя.</param>
    /// <param name="module">Модуль.</param>
    Task SendMessageToQueueAsync(string? message, long dialogId, string account, Guid userCode,
        UserConnectionModuleEnum module);
}