using LeokaEstetica.Platform.Communications.Models.Output;

namespace LeokaEstetica.Platform.Communications.Abstractions;

/// <summary>
/// Абстракция сервиса меню коммуникаций чата.
/// </summary>
public interface IChatCommunicationsMenuService
{
    /// <summary>
    /// Метод получает элементы меню для групп объектов чата.
    /// </summary>
    /// <returns>Элементы меню для групп объектов чата.</returns>
    Task<GroupObjectMenuOutput> GetGroupObjectMenuItemsAsync();
}