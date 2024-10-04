using Dapper;
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

    #region Публичные методы.

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

            if (result.Items is null || result.Items.Count == 0)
            {
                return new TopMenuOutput { Items = new List<TopItem>() };
            }

            // Перебираем все элементы и сортируем.
            foreach (var item in result.Items)
            {
                // Сортируем все вложенные элементы.
                if (item.Items is not null && item.Items.Count > 0)
                {
                    foreach (var item2 in item.Items)
                    {
                        if (item2.Items is not null && item2.Items.Count > 0)
                        {
                            foreach (var item3 in item2.Items)
                            {
                                item3.Items = item3.Items?.OrderBy(x => x.Position).AsList();
                            }

                            item2.Items = item2.Items?.OrderBy(x => x.Position).AsList();
                        }
                    }

                    item.Items = item.Items.OrderBy(x => x.Position).AsList();
                }
            }

            result.Items = result.Items.OrderBy(x => x.Position).AsList();

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<LeftMenuOutput> GetLeftMenuItemsAsync()
    {
        try
        {
            var json = await _menuRepository.GetLeftMenuItemsAsync();

            if (json is null)
            {
                throw new InvalidOperationException("Ошибка получения элементов левого меню.");
            }

            var result = JsonConvert.DeserializeObject<LeftMenuOutput>(json);

            if (result is null)
            {
                throw new InvalidOperationException("Ошибка при десериализации левого меню.");
            }

            if (result.Items is null || result.Items.Count == 0)
            {
                return new LeftMenuOutput { Items = new List<LeftItem>() };
            }

            // Перебираем все элементы и сортируем.
            foreach (var item in result.Items)
            {
                // Сортируем все вложенные элементы.
                if (item.Items is not null && item.Items.Count > 0)
                {
                    foreach (var item2 in item.Items)
                    {
                        if (item2.Items is not null && item2.Items.Count > 0)
                        {
                            foreach (var item3 in item2.Items)
                            {
                                item3.Items = item3.Items?.OrderBy(x => x.Position).AsList();
                            }
                        
                            item2.Items = item2.Items?.OrderBy(x => x.Position).AsList();
                        }
                    }

                    item.Items = item.Items.OrderBy(x => x.Position).AsList();
                }
            }

            result.Items = result.Items.OrderBy(x => x.Position).AsList();

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ProjectManagementLineMenuOutput> GetProjectManagementLineMenuAsync()
    {
        try
        {
            var json = await _menuRepository.GetProjectManagementLineMenuAsync();

            if (json is null)
            {
                throw new InvalidOperationException(
                    "Ошибка получения элементов меню области быстрых действий раб.пространства проекта.");
            }

            var result = JsonConvert.DeserializeObject<ProjectManagementLineMenuOutput>(json);

            if (result is null)
            {
                throw new InvalidOperationException(
                    "Ошибка при десериализации меню области быстрых действий раб.пространства проекта.");
            }

            if (result.Items is null || result.Items.Count == 0)
            {
                return new ProjectManagementLineMenuOutput { Items = new List<ProjectManagementLineMenuItem>() };
            }

            // Перебираем все элементы и сортируем.
            foreach (var item in result.Items)
            {
                // Сортируем все вложенные элементы.
                if (item.Items is not null && item.Items.Count > 0)
                {
                    foreach (var item2 in item.Items)
                    {
                        if (item2.Items is not null && item2.Items.Count > 0)
                        {
                            foreach (var item3 in item2.Items)
                            {
                                item3.Items = item3.Items?.OrderBy(x => x.Position).AsList();
                            }
                            
                            item2.Items = item2.Items?.OrderBy(x => x.Position).AsList();
                        }
                    }

                    item.Items = item.Items.OrderBy(x => x.Position).AsList();
                }
            }

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