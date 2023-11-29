using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
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
    public async Task<List<ProjectManagmentHeaderResult>> ModifyHeaderItemsAsync(
        IEnumerable<ProjectManagmentHeaderOutput> items)
    {
        try
        {
            var result = new List<ProjectManagmentHeaderResult>();

            // Будем наполнять доп.списки.
            foreach (var item in items)
            {
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
                        var strategyItems = mapItems.Items.OrderBy(o => o.Position);
                        
                        var selectedStrategyItems = strategyItems.Select(x => new ProjectManagmentHeader
                            {
                                Label = x.ItemName,
                                Id = ProjectManagmentDestinationTypeEnum.Strategy.ToString()
                            });
                        
                        result.Add(new ProjectManagmentHeaderResult
                        {
                            Label = item.ItemName,
                            Items = selectedStrategyItems
                        });
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
                        var createItems = mapItems.Items.OrderBy(o => o.Position);
                        
                        var selectedCreateItems = createItems.Select(x => new ProjectManagmentHeader
                        {
                            Label = x.ItemName,
                            Id = ProjectManagmentDestinationTypeEnum.Create.ToString()
                        });
                        
                        result.Add(new ProjectManagmentHeaderResult
                        {
                            Label = item.ItemName,
                            Items = selectedCreateItems
                        });
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
                        var filters = mapItems.Items.OrderBy(o => o.Position);
                        
                        var selectedFilters = filters.Select(x => new ProjectManagmentHeader
                            {
                                Label = x.ItemName,
                                Id = ProjectManagmentDestinationTypeEnum.Filters.ToString()
                            });
                        
                        result.Add(new ProjectManagmentHeaderResult
                        {
                            Label = item.ItemName,
                            Items = selectedFilters
                        });
                    }
                }
                
                // Если настройки.
                if (destination == ProjectManagmentDestinationTypeEnum.Settings)
                {
                    if (!item.HasItems)
                    {
                        result.Add(new ProjectManagmentHeaderResult
                        {
                            Label = item.ItemName
                        });
                    }

                    else
                    {
                        ProjectManagmentHeaderSettings mapItems;

                        try
                        {
                            mapItems = JsonConvert.DeserializeObject<ProjectManagmentHeaderSettings>(item.Items);
                        }
                    
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "Ошибка при десериализации элементов меню настроек." +
                                                  $" Settings: {item.Items}");
                            throw;
                        }
                    
                        if (mapItems is not null)
                        {
                            var settings = mapItems.Items.OrderBy(o => o.Position);
                        
                            var selectedSettings = settings.Select(x => new ProjectManagmentHeader
                                {
                                    Label = x.ItemName,
                                    Id = ProjectManagmentDestinationTypeEnum.Settings.ToString()
                                });
                        
                            result.AddRange(selectedSettings.Select(x => new ProjectManagmentHeaderResult
                            {
                                Label = x.Label,
                                Id = x.Id
                            }));
                        }
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

    /// <summary>
    /// Метод получает список шаблонов задач, которые пользователь может выбрать перед переходом в рабочее пространство.
    /// </summary>
    /// <returns>Список шаблонов задач.</returns>
    public async Task<IEnumerable<ProjectManagmentTaskTemplateEntityResult>> GetProjectManagmentTemplatesAsync()
    {
        try
        {
            var result = await _projectManagmentRepository.GetProjectManagmentTemplatesAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод проставляет Id шаблонов статусам для результата.
    /// </summary>
    /// <param name="templateStatuses">Список статусов.</param>
    public async Task SetProjectManagmentTemplateIdsAsync(List<ProjectManagmentTaskTemplateResult> templateStatuses)
    {
        try
        {
            if (templateStatuses is null || !templateStatuses.Any())
            {
                throw new InvalidOperationException(
                    "Невозможно проставить Id щаблонов статусам задач." +
                    $" TemplateStatuses: {JsonConvert.SerializeObject(templateStatuses)}");
            }

            // Находим в БД все статусы по их Id.
            var templateStatusIds = templateStatuses
                .SelectMany(x => x.ProjectManagmentTaskStatusTemplates
                    .Select(y => y.StatusId));
            var items = templateStatuses
                .SelectMany(x => x.ProjectManagmentTaskStatusTemplates
                    .Select(y => y));

            var statusesDict = await _projectManagmentRepository.GetTemplateStatusIdsByStatusIdsAsync(
                templateStatusIds);
            
            foreach (var ts in items)
            {
                var statusId = ts.StatusId;
                
                // Если не нашли такогго статуса в таблице маппинга многие-ко-многим.
                if (!statusesDict.ContainsKey(statusId))
                {
                    throw new InvalidOperationException(
                        $"Не удалось получить шаблон, к которому принадлежит статус. Id статуса был: {statusId}");
                }

                ts.TemplateId = statusesDict.TryGet(statusId);
            }
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}