using LeokaEstetica.Platform.Models.Dto.Communications.Output;

namespace LeokaEstetica.Platform.Communications.Abstractions;

/// <summary>
/// Абстракция сервиса диалогов.
/// </summary>
public interface IAbstractGroupDialogService
{
    /// <summary>
    /// Метод создает диалог и добавляет в него участников.
    /// </summary>
    /// <param name="memberEmails">Список участников диалога.</param>
    /// <param name="dialogName">Название диалога.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Выходная модель.</returns>
    Task<CreateDialogAndAddDialogMembersOutput> CreateDialogAndAddDialogMembersAsync(
        IEnumerable<string>? memberEmails, string? dialogName, string account);
}