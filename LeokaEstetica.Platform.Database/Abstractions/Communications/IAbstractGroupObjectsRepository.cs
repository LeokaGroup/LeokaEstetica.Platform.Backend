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
    Task<IEnumerable<GroupObjectDialogOutput>> GetObjectDialogsAsync(long abstractScopeId, long userId);
}