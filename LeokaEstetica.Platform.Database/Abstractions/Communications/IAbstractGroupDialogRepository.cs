using LeokaEstetica.Platform.Models.Dto.Communications.Output;

namespace LeokaEstetica.Platform.Database.Abstractions.Communications;

/// <summary>
/// Абстракция репозитория диалогов.
/// </summary>
public interface IAbstractGroupDialogRepository
{
    /// <summary>
    /// Метод создает диалог и добавляет в него участников.
    /// </summary>
    /// <param name="memberEmails">Список участников диалога.</param>
    /// <param name="dialogName">Название диалога.</param>
    /// <returns>Созданный диалог.</returns>
    Task<GroupObjectDialogOutput?> CreateDialogAndAddDialogMembersAsync(IEnumerable<long> memberIds,
        string? dialogName);
}