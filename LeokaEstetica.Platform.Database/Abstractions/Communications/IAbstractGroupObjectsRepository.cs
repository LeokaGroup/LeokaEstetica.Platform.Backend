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
    /// Метод получает список сообщений диалога.
    /// </summary>
    /// <param name="dialogId">Id диалога.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список сообщений диалога.</returns>
    Task<IEnumerable<GroupObjectDialogMessageOutput>> GetObjectDialogMessagesAsync(long dialogId, long userId);

    /// <summary>
    /// Метод получает список диалогов компании.
    /// </summary>
    /// <param name="companyId">Id компании.</param>
    /// <returns>Список диалогов компании.</returns>
    Task<IEnumerable<GroupObjectDialogOutput>> GetCompanyDialogsAsync(long companyId);
}