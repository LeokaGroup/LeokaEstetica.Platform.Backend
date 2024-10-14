using LeokaEstetica.Platform.Models.Dto.Communications.Output;

namespace LeokaEstetica.Platform.Communications.Abstractions;

/// <summary>
/// Абстракция сервиса объектов группы.
/// </summary>
public interface IAbstractGroupObjectsService
{
    /// <summary>
    /// Метод получает диалоги объекта группы.
    /// </summary>
    /// <param name="abstractScopeId">Id выбранной абстрактной области чата.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список диалогов с сообщениями.</returns>
    Task<IEnumerable<GroupObjectDialogOutput>> GetObjectDialogsAsync(long abstractScopeId, long userId);
}