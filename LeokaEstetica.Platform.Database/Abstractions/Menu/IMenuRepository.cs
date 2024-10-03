namespace LeokaEstetica.Platform.Database.Abstractions.Menu;

/// <summary>
/// Абстракция репозитория меню.
/// </summary>
public interface IMenuRepository
{
    /// <summary>
    /// Метод получает элементы верхнего меню.
    /// </summary>
    /// <returns>Элементы верхнего меню.</returns>
    Task<string?> GetTopMenuItemsAsync();
    
    /// <summary>
    /// Метод получает элементы левого меню.
    /// </summary>
    /// <returns>Элементы верхнего меню.</returns>
    Task<string?> GetLeftMenuItemsAsync();
}