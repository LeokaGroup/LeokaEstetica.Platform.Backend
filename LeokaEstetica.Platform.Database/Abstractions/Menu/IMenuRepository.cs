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
    
    /// <summary>
    /// Метод получает элементы меню для блока быстрых действий в раб.пространстве проекта.
    /// </summary>
    /// <returns>Элементы меню.</returns>
    Task<string?> GetProjectManagementLineMenuAsync();
    
    /// <summary>
    /// Метод получает элементы меню для всех Landing страниц.
    /// В будущем можно унифицировать этот эндпоинт будет под разные меню разных Landing страниц.
    /// </summary>
    /// <returns>Элементы Landing меню.</returns>
    Task<string?> GetLandingMenuItemsAsync();
    
    /// <summary>
    /// Метод получает элементы меню для групп объектов чата.
    /// </summary>
    /// <returns>Элементы меню для групп объектов чата.</returns>
    Task<string?> GetGroupObjectMenuItemsAsync();
    
    /// <summary>
    /// Метод получает элементы меню группировок диалогов чата.
    /// </summary>
    /// <returns>Элементы меню группировок диалогов чата.</returns>
    Task<string?> GetDialogGroupMenuItemsAsync();
}