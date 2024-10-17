using LeokaEstetica.Platform.Models.Dto.Communications.Output;

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
}