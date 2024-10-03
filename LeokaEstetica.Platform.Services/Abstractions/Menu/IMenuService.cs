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
    
    /// <summary>
    /// Метод получает элементы верхнего меню.
    /// </summary>
    /// <returns>Элементы верхнего меню.</returns>
    Task<LeftMenuOutput> GetLeftMenuItemsAsync();

    /// <summary>
    /// Метод получает элементы меню для блока быстрых действий в раб.пространстве проекта.
    /// </summary>
    /// <returns>Элементы меню.</returns>
    Task<ProjectManagementLineMenuOutput> GetProjectManagementLineMenuAsync();
}