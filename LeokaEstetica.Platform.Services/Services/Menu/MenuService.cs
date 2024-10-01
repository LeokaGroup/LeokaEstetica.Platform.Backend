using LeokaEstetica.Platform.Database.Abstractions.Menu;
using LeokaEstetica.Platform.Models.Dto.Output.Menu;
using LeokaEstetica.Platform.Services.Abstractions.Menu;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Services.Services.Menu;

/// <summary>
/// Класс реализует методы сервиса меню.
/// </summary>
internal sealed class MenuService : IMenuService
{
    private readonly ILogger<MenuService> _logger;
    private readonly IMenuRepository _menuRepository;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="menuRepository">Репозиторий меню.</param>
    public MenuService(ILogger<MenuService> logger,
        IMenuRepository menuRepository)
    {
        _logger = logger;
        _menuRepository = menuRepository;
    }

    /// <inheritdoc />
    public async Task<TopMenuOutput> GetTopMenuItemsAsync()
    {
        try
        {
            var json = await _menuRepository.GetTopMenuItemsAsync();
            
            if (json is null)
            {
                throw new InvalidOperationException("Ошибка получения элементов верхнего меню.");
            }
            
            var result = JsonConvert.DeserializeObject<TopMenuOutput>(json);

            if (result is null)
            {
                throw new InvalidOperationException("Ошибка при десериализации верхнего меню.");
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }
}