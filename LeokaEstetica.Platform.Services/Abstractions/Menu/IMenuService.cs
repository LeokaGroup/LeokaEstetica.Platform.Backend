using LeokaEstetica.Platform.Models.Dto.Output.Menu;

namespace LeokaEstetica.Platform.Services.Abstractions.Menu;

/// <summary>
/// Абстракция сервиса меню.
/// </summary>
public interface IMenuService
{
    /// <summary>
    /// Метод получает элементы верхнего меню.
    /// </summary>
    /// <returns>Элементы верхнего меню.</returns>
    Task<TopMenuOutput> GetTopMenuItemsAsync();
}