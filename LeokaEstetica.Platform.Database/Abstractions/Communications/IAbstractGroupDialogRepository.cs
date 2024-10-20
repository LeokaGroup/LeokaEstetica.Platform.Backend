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

    /// <summary>
    /// Метод добавляет сообщение чата в БД.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="createdBy">Кто создал сообщение.</param>
    /// <param name="dialogId">Id диалога.</param>
    /// <returns>Добавленное сообщение.</returns>
    Task<GroupObjectDialogMessageOutput?> SaveMessageAsync(string? message, long createdBy, long dialogId);
}