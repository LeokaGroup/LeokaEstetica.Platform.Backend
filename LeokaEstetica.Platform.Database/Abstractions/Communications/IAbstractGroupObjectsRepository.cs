using LeokaEstetica.Platform.Models.Dto.Communications.Output;

namespace LeokaEstetica.Platform.Database.Abstractions.Communications;

/// <summary>
/// Абстракция репозитория объектов группы.
/// </summary>
public interface IAbstractGroupObjectsRepository
{
    /// <summary>
    /// Метод получает диалоги объекта группы.
    /// </summary>
    /// <param name="abstractScopeId">Id выбранной абстрактной области чата.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список диалогов с сообщениями.</returns>
    Task<IEnumerable<GroupObjectDialogMessageOutput>> GetObjectDialogsAsync(long abstractScopeId, long userId);
    
    /// <summary>
    /// Метод получает сообщения объектов.
    /// </summary>
    /// <param name="objectIds">Список Id объектов.</param>
    /// <returns>Словарь с сообщениями диалогов объектов.</returns>
    Task<IEnumerable<GroupObjectDialogMessageOutput>> GetObjectDialogMessagesAsync(IEnumerable<long> objectIds);
}