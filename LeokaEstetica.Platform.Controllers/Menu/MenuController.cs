using LeokaEstetica.Platform.Base;
using LeokaEstetica.Platform.Base.Filters;
using LeokaEstetica.Platform.Models.Dto.Output.Menu;
using LeokaEstetica.Platform.Services.Abstractions.Menu;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Controllers.Menu;

/// <summary>
/// Контроллер разных меню.
/// </summary>
[AuthFilter]
[ApiController]
[Route("menu")]
public class MenuController : BaseController
{
    private readonly IMenuService _menuService;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="menuService">Сервис меню.</param>
    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// Метод получает элементы верхнего меню.
    /// </summary>
    /// <returns>Элементы верхнего меню.</returns>
    [HttpGet]
    [Route("top-menu")]
    [ProducesResponseType(200, Type = typeof(TopMenuOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<TopMenuOutput> GetTopMenuItemsAsync()
    {
        var result = await _menuService.GetTopMenuItemsAsync();

        return result;
    }
    
    /// <summary>
    /// Метод получает элементы верхнего меню.
    /// </summary>
    /// <returns>Элементы верхнего меню.</returns>
    [HttpGet]
    [Route("left-menu")]
    [ProducesResponseType(200, Type = typeof(TopMenuOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<LeftMenuOutput> GetLeftMenuItemsAsync()
    {
        var result = await _menuService.GetLeftMenuItemsAsync();

        return result;
    }

    /// <summary>
    /// Метод получает элементы меню для блока быстрых действий в раб.пространстве проекта.
    /// </summary>
    /// <returns>Элементы меню.</returns>
    [HttpGet]
    [Route("project-management-line-menu")]
    [ProducesResponseType(200, Type = typeof(ProjectManagementLineMenuOutput))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [ProducesResponseType(404)]
    public async Task<ProjectManagementLineMenuOutput> GetProjectManagementLineMenuAsync()
    {
        var result = await _menuService.GetProjectManagementLineMenuAsync();

        return result;
    }
}