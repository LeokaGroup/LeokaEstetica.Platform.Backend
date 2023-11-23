using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса управления проектами.
/// </summary>
internal sealed class ProjectManagmentService : IProjectManagmentService
{
    private readonly ILogger<ProjectManagmentService> _logger;
    private readonly IProjectManagmentRepository _projectManagmentRepository;
    
    /// <summary>
    /// Конструктор.
    /// <param name="logger">Логгер.</param>
    /// <param name="projectManagmentRepository">Репозиторий управления проектами.</param>
    /// </summary>
    public ProjectManagmentService(ILogger<ProjectManagmentService> logger,
        IProjectManagmentRepository projectManagmentRepository)
    {
        _logger = logger;
        _projectManagmentRepository = projectManagmentRepository;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список стратегий представления рабочего пространства.
    /// </summary>
    /// <returns>Список стратегий.</returns>
    public async Task<IEnumerable<ViewStrategyEntity>> GetViewStrategiesAsync()
    {
        try
        {
            var result = await _projectManagmentRepository.GetViewStrategiesAsync();

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает элементы верхнего меню (хидера).
    /// Этот метод не заполняет доп.списки.
    /// </summary>
    /// <returns>Список элементов.</returns>
    public async Task<IEnumerable<ProjectManagmentHeaderEntity>> GetHeaderItemsAsync()
    {
        try
        {
            var result = await _projectManagmentRepository.GetHeaderItemsAsync();

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при получении элементов верхнего меню (хидера).");
            throw;
        }
    }

    /// <summary>
    /// Метод наполняет доп.списки элементов хидера.
    /// </summary>
    /// <param name="items">Список элементов.</param>
    public async Task<ProjectManagmentHeaderResult> ModifyHeaderItemsAsync(
        IEnumerable<ProjectManagmentHeaderOutput> items)
    {
        try
        {
            var result = new ProjectManagmentHeaderResult
            {
                ProjectManagmentHeaderItems = items
            };
            
            // Будем наполнять доп.списки.
            foreach (var item in result.ProjectManagmentHeaderItems)
            {
                // Если нет доп.элементов, то нам не интересен этот пункт меню.
                if (!item.HasItems)
                {
                    continue;
                }

                var destinationString = item.Destination;
                var destination = Enum.Parse<ProjectManagmentDestinationTypeEnum>(destinationString);

                if (destination == ProjectManagmentDestinationTypeEnum.None)
                {
                    throw new InvalidOperationException("Неизвестное назначение элемента меню." +
                                                        $"Элемент меню: {destinationString}");
                }

                // Если список стратегий представления.
                if (destination == ProjectManagmentDestinationTypeEnum.Strategy)
                {
                    ProjectManagmentHeaderStrategyItems mapItems;

                    try
                    {
                        mapItems = JsonConvert.DeserializeObject<ProjectManagmentHeaderStrategyItems>(item.Items);
                    }
                    
                    catch (JsonSerializationException ex)
                    {
                        _logger?.LogError(ex, "Ошибка при десериализации элементов меню стратегий представления." +
                                              $" StrategyItems: {item.Items}");
                        throw;
                    }

                    if (mapItems is not null)
                    {
                        result.StrategyItems = mapItems.Items.OrderBy(o => o.Position);
                    }
                }

                // Если список кнопка создания.
                if (destination == ProjectManagmentDestinationTypeEnum.Create)
                {
                    ProjectManagmentHeaderCreateItems mapItems;

                    try
                    {
                        mapItems = JsonConvert.DeserializeObject<ProjectManagmentHeaderCreateItems>(item.Items);
                    }
                    
                    catch (JsonSerializationException ex)
                    {
                        _logger?.LogError(ex, "Ошибка при десериализации элементов меню кнопки создания." +
                                              $" CreateItems: {item.Items}");
                        throw;
                    }

                    if (mapItems is not null)
                    {
                        result.CreateItems = mapItems.Items.OrderBy(o => o.Position);
                    }
                }

                // Если фильтры.
                if (destination == ProjectManagmentDestinationTypeEnum.Filters)
                {
                    ProjectManagmentHeaderFilters mapItems;

                    try
                    {
                        mapItems = JsonConvert.DeserializeObject<ProjectManagmentHeaderFilters>(item.Items);
                    }
                    
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Ошибка при десериализации элементов меню фильтров." +
                                              $" Filters: {item.Items}");
                        throw;
                    }
                    
                    if (mapItems is not null)
                    {
                        result.Filters = mapItems.Items.OrderBy(o => o.Position);
                    }
                }
            }

            return await Task.FromResult(result);
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при наполнении доп.списка элементов верхнего меню (хидера).");
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}