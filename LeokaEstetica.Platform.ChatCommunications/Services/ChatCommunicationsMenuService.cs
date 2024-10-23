using Dapper;
using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Communications.Models.Output;
using LeokaEstetica.Platform.Database.Abstractions.Menu;
using LeokaEstetica.Platform.Models.Dto.Base.Menu;
using LeokaEstetica.Platform.Services.Abstractions.Menu;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Communications.Services;

/// <summary>
/// Класс реализует методы сервиса меню коммуникаций чата.
/// </summary>
internal sealed class ChatCommunicationsMenuService : IChatCommunicationsMenuService
{
    private readonly IMenuService _menuService;
    private readonly IMenuRepository _menuRepository;
    private readonly ILogger<ChatCommunicationsMenuService> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="menuService">Сервис элементов меню.</param>
    /// <param name="menuRepository">Репозиторий элементов меню.</param>
    /// <param name="logger">Логгер.</param>
    public ChatCommunicationsMenuService(IMenuService menuService,
        IMenuRepository menuRepository,
        ILogger<ChatCommunicationsMenuService> logger)
    {
        _menuService = menuService;
        _menuRepository = menuRepository;
        _logger = logger;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task<GroupObjectMenuOutput> GetGroupObjectMenuItemsAsync()
    {
        try
        {
            var json = await _menuRepository.GetGroupObjectMenuItemsAsync();

            if (json is null)
            {
                throw new InvalidOperationException("Ошибка получения элементов меню для групп объектов чата.");
            }

            var result = JsonConvert.DeserializeObject<GroupObjectMenuOutput>(json);

            if (result is null)
            {
                throw new InvalidOperationException(
                    "Ошибка при десериализации элементов меню для групп объектов чата.");
            }

            if (result.Items is null || result.Items.Count == 0)
            {
                return new GroupObjectMenuOutput { Items = new List<MenuItem>() };
            }
            
            await _menuService.SortingMenuItemsAsync(result.Items);

            result.Items = result.Items.OrderBy(x => x.Position).AsList();

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}