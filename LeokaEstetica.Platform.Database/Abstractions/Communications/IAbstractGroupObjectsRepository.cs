using LeokaEstetica.Platform.Models.Dto.Communications.Output;

namespace LeokaEstetica.Platform.Database.Abstractions.Communications;

/// <summary>
/// Абстракция репозитория объектов группы.
/// </summary>
public interface IAbstractGroupObjectsRepository
{
    /// <summary>
    /// Метод получает диалоги объектов.
    /// </summary>
    /// <param name="objectIds">Список Id объектов.</param>
    /// <returns>Словарь с сообщениями диалогов объектов.</returns>
    Task<IEnumerable<GroupObjectDialogOutput>> GetObjectDialogsAsync(IEnumerable<long> objectIds);

    /// <summary>
    /// Метод получает список сообщений диалогов.
    /// </summary>
    /// <param name="dialogIds">Список Id диалогов.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список сообщений диалогов.</returns>
    Task<IEnumerable<GroupObjectDialogMessageOutput>> GetObjectDialogMessagesAsync(IEnumerable<long> dialogIds,
        long userId);
}