using System.Diagnostics;
using System.Runtime.CompilerServices;
using AutoMapper;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Extensions.StringExtensions;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Database.Abstractions.Template;
using LeokaEstetica.Platform.Integrations.Abstractions.Pachca;
using LeokaEstetica.Platform.Integrations.Abstractions.Reverso;
using LeokaEstetica.Platform.Models.Dto.Base.ProjectManagement.Paginator;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Dto.Output.User;
using LeokaEstetica.Platform.Models.Dto.ProjectManagement.Output;
using LeokaEstetica.Platform.Models.Entities.Document;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.Template;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.ProjectManagment.Documents.Abstractions;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Services.Abstractions.User;
using LeokaEstetica.Platform.Services.Builders.AgileObjectBuilder;
using LeokaEstetica.Platform.Services.Builders.BuilderData;
using LeokaEstetica.Platform.Services.Factors;
using LeokaEstetica.Platform.Services.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Enum = System.Enum;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса управления проектами.
/// </summary>
internal sealed class ProjectManagmentService : IProjectManagmentService
{
    private readonly ILogger<ProjectManagmentService> _logger;
    private readonly IProjectManagmentRepository _projectManagmentRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IPachcaService _pachcaService;
    private readonly IProjectManagmentTemplateRepository _projectManagmentTemplateRepository;
    private readonly ITransactionScopeFactory _transactionScopeFactory;
    private readonly IProjectSettingsConfigRepository _projectSettingsConfigRepository;
    private readonly Lazy<IReversoService> _reversoService;
    private readonly Lazy<IFileManagerService> _fileManagerService;
    private readonly Lazy<IProjectManagementNotificationService> _projectManagementNotificationService;
    private readonly IUserService _userService;

    /// <summary>
    /// Статусы задач, которые являются самыми базовыми и никогда не меняются независимо от шаблона проекта.
    /// Новые статусы обязательно должны ассоциироваться с одним из перечисленных системных названий.
    /// </summary>
    private readonly List<string> _associationStatusSysNames = new()
    {
        "New", "InWork", "InDevelopment", "Completed"
    };

    /// <summary>
    /// Кол-во задач у статуса. Если применяется Scrum.
    /// </summary>
    private const int _scrumPageSize = 10;

    /// <summary>
    /// Конструктор.
    /// <param name="logger">Логгер.</param>
    /// <param name="projectManagmentRepository">Репозиторий управления проектами.</param>
    /// <param name="mapper">Маппер.</param>
    /// <param name="mapper">Репозиторий пользователей.</param>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    /// <param name="pachcaService">Сервис уведомлений пачки.</param>
    /// <param name="projectManagmentTemplateRepository">Репозиторий шаблонов проектов.</param>
    /// <param name="transactionScopeFactory">Факторка транзакций.</param>
    /// <param name="projectSettingsConfigRepository">Репозиторий настроек проектов.</param>
    /// <param name="projectSettingsConfigRepository">Сервис транслитера.</param>
    /// <param name="fileManagerService">Сервис менеджера файлов.</param>
    /// <param name="sprintNotificationsService">Сервис уведомлений спринтов.</param>
    /// <param name="userService">Сервис пользователей.</param>
    /// </summary>
    public ProjectManagmentService(ILogger<ProjectManagmentService> logger,
        IProjectManagmentRepository projectManagmentRepository,
        IMapper mapper,
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        IPachcaService pachcaService,
        IProjectManagmentTemplateRepository projectManagmentTemplateRepository,
        ITransactionScopeFactory transactionScopeFactory,
        IProjectSettingsConfigRepository projectSettingsConfigRepository,
        Lazy<IReversoService> reversoService,
        Lazy<IFileManagerService> fileManagerService,
        Lazy<IProjectManagementNotificationService> projectManagementNotificationService,
        IUserService userService)
    {
        _logger = logger;
        _projectManagmentRepository = projectManagmentRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _pachcaService = pachcaService;
        _projectManagmentTemplateRepository = projectManagmentTemplateRepository;
        _transactionScopeFactory = transactionScopeFactory;
        _projectSettingsConfigRepository = projectSettingsConfigRepository;
        _reversoService = reversoService;
        _fileManagerService = fileManagerService;
        _projectManagementNotificationService = projectManagementNotificationService;
        _userService = userService;
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
    public async Task<IEnumerable<PanelEntity>> GetPanelItemsAsync()
    {
        try
        {
            var result = await _projectManagmentRepository.GetPanelItemsAsync();

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, "Ошибка при получении элементов меню панели.");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<GetPanelResult> ModifyPanelItemsAsync(IEnumerable<PanelOutput> items)
    {
        try
        {
            var result = new GetPanelResult
            {
                HeaderItems = new List<PanelResult>(),
                PanelItems = new List<PanelResult>()
            };

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

                        var selectedStrategyItems = strategyItems.Select(x => new Panel
                        {
                            Label = x.ItemName,
                            Id = x.Id,
                            Disabled = x.Disabled,
                            IsFooterItem = x.IsFooterItem
                        });

                        result.HeaderItems.Add(new PanelResult
                        {
                            Label = item.ItemName,
                            Items = selectedStrategyItems,
                            Id = ProjectManagmentDestinationTypeEnum.Strategy.ToString(),
                            Disabled = item.IsDisabled,
                            IsFooterItem = item.IsFooterItem
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

                        var selectedCreateItems = createItems.Select(x => new Panel
                        {
                            Label = x.ItemName,
                            Id = x.Id,
                            Disabled = x.Disabled,
                            IsFooterItem = x.IsFooterItem
                        });

                        result.HeaderItems.Add(new PanelResult
                        {
                            Label = item.ItemName,
                            Items = selectedCreateItems,
                            Id = ProjectManagmentDestinationTypeEnum.Create.ToString(),
                            Disabled = item.IsDisabled,
                            IsFooterItem = item.IsFooterItem
                        });
                    }
                }

                // Если фильтры.
                if (destination == ProjectManagmentDestinationTypeEnum.Filters)
                {
                    ProjectManagmentPanelFilters mapItems;

                    try
                    {
                        mapItems = JsonConvert.DeserializeObject<ProjectManagmentPanelFilters>(item.Items);
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

                        var selectedFilters = filters.Select(x => new Panel
                        {
                            Label = x.ItemName,
                            Id = x.Id,
                            Disabled = x.Disabled,
                            IsFooterItem = x.IsFooterItem
                        });

                        result.HeaderItems.Add(new PanelResult
                        {
                            Label = item.ItemName,
                            Items = selectedFilters,
                            Id = ProjectManagmentDestinationTypeEnum.Filters.ToString(),
                            Disabled = item.IsDisabled,
                            IsFooterItem = item.IsFooterItem
                        });
                    }
                }

                // Если настройки.
                if (destination == ProjectManagmentDestinationTypeEnum.Settings)
                {
                    PanelSettings mapItems;

                    try
                    {
                        mapItems = JsonConvert.DeserializeObject<PanelSettings>(item.Items);
                    }

                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Ошибка при десериализации элементов меню настроек." +
                                              $" Filters: {item.Items}");
                        throw;
                    }

                    if (mapItems is not null)
                    {
                        var filters = mapItems.Items.OrderBy(o => o.Position);

                        var selectedFilters = filters.Select(x => new Panel
                        {
                            Label = x.ItemName,
                            Id = x.Id,
                            Disabled = x.Disabled,
                            IsFooterItem = x.IsFooterItem
                        });

                        result.HeaderItems.Add(new PanelResult
                        {
                            Label = item.ItemName,
                            Items = selectedFilters,
                            Id = ProjectManagmentDestinationTypeEnum.Settings.ToString(),
                            Disabled = item.IsDisabled,
                            IsFooterItem = item.IsFooterItem
                        });
                    }
                }
                
                // Если левая панель.
                if (destination == ProjectManagmentDestinationTypeEnum.LeftPanel)
                {
                    ProjectManagmentPanelFilters mapItems;

                    try
                    {
                        mapItems = JsonConvert.DeserializeObject<ProjectManagmentPanelFilters>(item.Items);
                    }

                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Ошибка при десериализации элементов меню левой панели." +
                                              $" Filters: {item.Items}");
                        throw;
                    }

                    if (mapItems is not null)
                    {
                        var filters = mapItems.Items.OrderBy(o => o.Position);

                        var selectedFilters = filters.Select(x => new Panel
                        {
                            Label = x.ItemName,
                            Id = x.Id,
                            Disabled = x.Disabled,
                            IsFooterItem = x.IsFooterItem
                        });

                        result.PanelItems.Add(new PanelResult
                        {
                            Label = item.ItemName,
                            Items = selectedFilters,
                            Id = ProjectManagmentDestinationTypeEnum.LeftPanel.ToString(),
                            Disabled = item.IsDisabled,
                            IsFooterItem = item.IsFooterItem
                        });
                    }
                }
                
                // Если список экспорта.
                if (destination == ProjectManagmentDestinationTypeEnum.Export)
                {
                    ProjectManagmentHeaderExportItems mapItems;

                    try
                    {
                        mapItems = JsonConvert.DeserializeObject<ProjectManagmentHeaderExportItems>(item.Items);
                    }

                    catch (JsonSerializationException ex)
                    {
                        _logger?.LogError(ex, "Ошибка при десериализации элементов меню экспорта." +
                                              $" ExportItems: {item.Items}");
                        throw;
                    }

                    if (mapItems is not null)
                    {
                        var exportItems = mapItems.Items.OrderBy(o => o.Position);

                        var selectedExportItems = exportItems.Select(x => new Panel
                        {
                            Label = x.ItemName,
                            Id = x.Id,
                            Disabled = x.Disabled,
                            IsFooterItem = x.IsFooterItem
                        });

                        result.HeaderItems.Add(new PanelResult
                        {
                            Label = item.ItemName,
                            Items = selectedExportItems,
                            Id = ProjectManagmentDestinationTypeEnum.Export.ToString(),
                            Disabled = item.IsDisabled,
                            IsFooterItem = item.IsFooterItem
                        });
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
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Список шаблонов задач.</returns>
    public async Task<IEnumerable<ProjectManagmentTaskTemplateResult>> GetProjectManagmentTemplatesAsync(
        long? templateId)
    {
        try
        {
            var items = (await _projectManagmentRepository.GetProjectManagmentTemplatesAsync(templateId))
                .ToList();
            
            // Разбиваем статусы на группы (кажда группа это отдельный шаблон со статусами).
            var templateIds = items.Select(x => x.TemplateId).Distinct().ToList();
            var result = new List<ProjectManagmentTaskTemplateResult>();

            foreach (var tid in templateIds)
            {
                // Выбираем все статусы определенного шаблона и добавляем в результат. 
                var templateStatuses = items.Where(x => x.TemplateId == tid);
                var resultItem = new ProjectManagmentTaskTemplateResult
                {
                    TemplateName = templateStatuses.First().TemplateName,
                    ProjectManagmentTaskStatusTemplates =
                        _mapper.Map<IEnumerable<ProjectManagmentTaskStatusTemplateOutput>>(templateStatuses)
                };
                result.Add(resultItem);
            }

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
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

            var statuses = (await _projectManagmentRepository.GetTemplateStatusIdsByStatusIdsAsync(templateStatusIds))
                .ToList();

            foreach (var ts in items)
            {
                var statusId = ts.StatusId;
                var templateData = statuses.Find(x => x.StatusId == statusId);

                // Если не нашли такого статуса в таблице маппинга многие-ко-многим.
                if (templateData is null || templateData.StatusId <= 0 || templateData.TemplateId <= 0)
                {
                    throw new InvalidOperationException(
                        $"Не удалось получить шаблон, к которому принадлежит статус. Id статуса был: {statusId}");
                }

                ts.TemplateId = templateData.TemplateId;
            }
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SetProjectManagmentTemplateIdsAsync(List<TaskStatusOutput> templateStatuses)
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
            var templateStatusIds = templateStatuses.Select(x => x.StatusId);

            var statuses = (await _projectManagmentRepository.GetTemplateStatusIdsByStatusIdsAsync(templateStatusIds))
                .ToList();

            foreach (var ts in templateStatuses)
            {
                var statusId = ts.StatusId;
                var templateData = statuses.Find(x => x.StatusId == statusId);

                // Если не нашли такого статуса в таблице маппинга многие-ко-многим.
                if (templateData is null || templateData.StatusId <= 0 || templateData.TemplateId <= 0)
                {
                    throw new InvalidOperationException(
                        $"Не удалось получить шаблон, к которому принадлежит статус. Id статуса был: {statusId}");
                }

                ts.TemplateId = templateData.TemplateId;
            }
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ProjectManagmentWorkspaceResult> GetConfigurationWorkSpaceBySelectedTemplateAsync(long projectId,
        string account, int? paginatorStatusId, ModifyTaskStatuseTypeEnum modifyTaskStatuseType, int page = 1)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            // TODO: Этот код дублируется в этом сервисе. Вынести в приватный метод и кортежем вернуть нужные данные.
            // Получаем настройки проекта.
            var projectSettings = await _projectSettingsConfigRepository.GetProjectSpaceSettingsByProjectIdAsync(
                projectId, userId);
            var projectSettingsItems = projectSettings?.AsList();

            if (projectSettingsItems is null
                || !projectSettingsItems.Any()
                || projectSettingsItems.Any(x => x is null))
            {
                throw new InvalidOperationException("Ошибка получения настроек проекта. " +
                                                    $"ProjectId: {projectId}. " +
                                                    $"UserId: {userId}");
            }

            var template = projectSettingsItems.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_TEMPLATE_ID));
            var templateId = Convert.ToInt32(template!.ParamValue);

            // Получаем набор статусов, которые входят в выбранный шаблон.
            var items = await GetProjectManagmentTemplatesAsync(templateId);
            var templateStatusesItems = _mapper.Map<IEnumerable<ProjectManagmentTaskTemplateResult>>(items);
            var statuses = templateStatusesItems?.AsList();

            if (statuses is null || !statuses.Any())
            {
                throw new InvalidOperationException("Не удалось получить набор статусов шаблона." +
                                                    $" TemplateId: {templateId}." +
                                                    $"ProjectId: {projectId}.");
            }

            // Проставляем Id шаблона статусам.
            await SetProjectManagmentTemplateIdsAsync(statuses);
            
            // TODO: Получать сохраненную стратегию пользователя, а не всего проекта.
            var strategy = projectSettingsItems.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_STRATEGY));

            var result = new ProjectManagmentWorkspaceResult
            {
                ProjectManagmentTaskStatuses = statuses.First().ProjectManagmentTaskStatusTemplates
                    .Where(x => x.TemplateId == templateId),
                Strategy = strategy!.ParamValue
            };
            
            var modifyStatusesTimer = new Stopwatch();
                
            _logger?.LogInformation(
                $"Начали получение списка задач для рабочего пространства для проекта {projectId}");
                
            modifyStatusesTimer.Start();

            // Получаем задачи пользователя, которые принадлежат проекту в рабочем пространстве.
            var projectTasks = await _projectManagmentRepository.GetProjectTasksAsync(projectId, strategy!.ParamValue);
            
            modifyStatusesTimer.Stop();
                
            _logger?.LogInformation(
                $"Закончили получение списка задач для рабочего пространства для проекта {projectId} " +
                $"за: {modifyStatusesTimer.ElapsedMilliseconds} мсек.");
            
            var tasks = projectTasks?.AsList();

            // Если задачи есть, то модифицируем выходные данные.
            if (tasks is not null && tasks.Any())
            {
                // Исключаем некоторые статусы из бэклога, так как из них нельзя спланировать спринт.
                if (modifyTaskStatuseType == ModifyTaskStatuseTypeEnum.Backlog)
                {
                    var excludedStatuses = new[]
                    {
                        (long)ProjectTaskStatusEnum.Completed,
                        (long)ProjectTaskStatusEnum.Archive,
                        (long)ProjectTaskStatusEnum.Closed
                    };
                
                    result.ProjectManagmentTaskStatuses = result.ProjectManagmentTaskStatuses
                        .Where(x => !excludedStatuses.Contains(x.TaskStatusId));
                    
                    tasks = tasks.Where(x => !excludedStatuses.Contains(x.TaskStatusId)).AsList();
                }
                
                await ModifyProjectManagmentTaskStatusesResultAsync(result.ProjectManagmentTaskStatuses, tasks,
                    projectId, result.Strategy, paginatorStatusId, page);
            }

            return result;
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ProjectManagmentTaskOutput> GetTaskDetailsByTaskIdAsync(string projectTaskId, string account,
        long projectId, TaskDetailTypeEnum taskDetailType)
    {
        try
        {
            if (taskDetailType == TaskDetailTypeEnum.None)
            {
                throw new InvalidOperationException("Неизвестный тип детализации. " +
                                                    " Заполнение Agile-объекта не будет происходить.");
            }
            
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            // TODO: Добавить проверку. Является ли пользователь участником проекта. Если нет, то не давать доступ к задаче.
            var builderData = new AgileObjectBuilderData(_projectManagmentRepository, _userRepository,
                _pachcaService, _userService, _projectManagmentTemplateRepository, _mapper,
                projectTaskId.GetProjectTaskIdFromPrefixLink(), projectId);
            AgileObjectBuilder builder = null;

            // Если просматриваем задачу.
            if (taskDetailType is TaskDetailTypeEnum.Task or TaskDetailTypeEnum.Error)
            {
                // Настраиваем билдер для построения задачи.
                builder = new TaskBuilder { BuilderData = builderData };
            }
            
            // Если просматриваем эпик.
            if (taskDetailType is TaskDetailTypeEnum.Epic)
            {
                // Настраиваем билдер для построения эпика.
                builder = new EpicBuilder { BuilderData = builderData };
            }
            
            // Если просматриваем историю.
            if (taskDetailType is TaskDetailTypeEnum.History)
            {
                // Настраиваем билдер для построения истории.
                builder = new UserStory { BuilderData = builderData };
            }

            if (builder is null)
            {
                throw new InvalidOperationException("Тип билдера не определен. Построения не будет происходить. " +
                                                    $"TaskDetailTypeEnum: {taskDetailType}");
            }
            
            var agileObject = new AgileObject();

            // Запускаем построение нужного Agile-объекта.
            await agileObject.BuildAsync(builder, taskDetailType);
                
            return builder.ProjectManagmentTask;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }
    
    /// <inheritdoc />
    public async Task<CreateProjectManagementTaskOutput> CreateProjectTaskAsync(
        CreateProjectManagementTaskInput projectManagementTaskInput, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var projectId = projectManagementTaskInput.ProjectId;
            
            // TODO: Этот код дублируется в этом сервисе. Вынести в приватный метод и кортежем вернуть нужные данные.
            // Получаем настройки проекта.
            var projectSettings = await _projectSettingsConfigRepository.GetProjectSpaceSettingsByProjectIdAsync(
                projectId, userId);
            var projectSettingsItems = projectSettings?.AsList();

            if (projectSettingsItems is null || !projectSettingsItems.Any())
            {
                throw new InvalidOperationException("Ошибка получения настроек проекта. " +
                                                    $"ProjectId: {projectId}. " +
                                                    $"UserId: {userId}");
            }
            var redirectUrl = projectSettingsItems.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_SPACE_URL));

            var result = new CreateProjectManagementTaskOutput
            {
                RedirectUrl = string.Concat(redirectUrl!.ParamValue, $"?projectId={projectId}")
            };

            var parseTaskType = Enum.GetName((ProjectTaskTypeEnum)projectManagementTaskInput.TaskTypeId);
            var taskType = Enum.Parse<ProjectTaskTypeEnum>(parseTaskType!);

            // Находим наибольший Id задачи в рамках проекта и увеличиваем его.
            /*
            Описание алгоритма:
                1. При создании задачи, вне зависимости от ее типа, идти брать максимальное значение поля
                 project_task_id  из всех этих таблиц project_tasks, epics, user_stories.
                2. Считать максимальное из этих взятых чисел.
                3. Писать project_task_id в нужную таблицу с этим значением (оно и будет самым актуальным Id задачи
            в рамках проекта).
            */
            var maxProjectTaskId = await _projectManagmentRepository.GetLastProjectTaskIdAsync(projectId);

            // Если идет создание задачи или ошибки.
            if (new[] { ProjectTaskTypeEnum.Task, ProjectTaskTypeEnum.Error }.Contains(taskType))
            {
                if (maxProjectTaskId < 0)
                {
                    throw new InvalidOperationException("Не удалось получить наибольший Id задачи в рамках проекта." +
                                                        $"ProjectId: {projectId}");
                }

                // Если не было добавленной записи, то начнем с 1.
                if (maxProjectTaskId == 0)
                {
                    maxProjectTaskId++;
                }

                ProjectTaskEntity addedProjectTask;

                using var transactionScope = _transactionScopeFactory.CreateTransactionScope();

                // Ускоренное создание задачи.
                if (projectManagementTaskInput.IsQuickCreate)
                {
                    // TODO: Для чего вообще использовать класс сущности?
                    // TODO: С Dapper не нужно все это.
                    // TODO: Использовать просто классы DTO для этого, и факторки эти не нужны будут.
                    addedProjectTask = CreateProjectTaskFactory.CreateQuickProjectTask(projectManagementTaskInput,
                        userId, ++maxProjectTaskId);
                }

                // Обычное создание задачи.
                else
                {
                    // Если не был указан исполнитель задачи, то исполнителем задачи будет ее автор.
                    if (!projectManagementTaskInput.ExecutorId.HasValue)
                    {
                        projectManagementTaskInput.ExecutorId = userId;
                    }

                    // TODO: Для чего вообще использовать класс сущности?
                    // TODO: С Dapper не нужно все это.
                    // TODO: Использовать просто классы DTO для этого, и факторки эти не нужны будут.
                    addedProjectTask = CreateProjectTaskFactory.CreateProjectTask(projectManagementTaskInput, userId,
                        ++maxProjectTaskId);
                }

                // Создаем задачу в БД.
                await _projectManagmentRepository.CreateProjectTaskAsync(addedProjectTask);

                transactionScope.Complete();
                
                return result;
            }
            
            // Если идет создание эпика.
            if (taskType == ProjectTaskTypeEnum.Epic)
            {
                using var transactionScope = _transactionScopeFactory.CreateTransactionScope();
                
                // TODO: Для чего вообще использовать класс сущности?
                // TODO: С Dapper не нужно все это.
                // TODO: Использовать просто классы DTO для этого, и факторки эти не нужны будут.
                var addedProjectEpic = CreateProjectTaskFactory.CreateProjectEpic(projectManagementTaskInput, userId,
                    ++maxProjectTaskId);
                
                // Создаем эпик в БД.
                await _projectManagmentRepository.CreateProjectEpicAsync(addedProjectEpic);
                
                transactionScope.Complete();
                
                return result;
            }
            
            // Если идет создание истории/требования.
            if (taskType == ProjectTaskTypeEnum.History)
            {
                using var transactionScope = _transactionScopeFactory.CreateTransactionScope();
                
                // TODO: Для чего вообще использовать класс сущности?
                // TODO: С Dapper не нужно все это.
                // TODO: Использовать просто классы DTO для этого, и факторки эти не нужны будут.
                var addedProjectUserStory = CreateProjectTaskFactory.CreateProjectUserStory(projectManagementTaskInput,
                    userId, ++maxProjectTaskId);
                
                // Создаем историю в БД.
                await _projectManagmentRepository.CreateProjectUserStoryAsync(addedProjectUserStory);
                
                transactionScope.Complete();
                
                return result;
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список приоритетов задачи.
    /// </summary>
    /// <returns>Список приоритетов задачи.</returns>
    public async Task<IEnumerable<TaskPriorityEntity>> GetTaskPrioritiesAsync()
    {
        try
        {
            var result = await _projectManagmentRepository.GetTaskPrioritiesAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список типов задач.
    /// </summary>
    /// <returns>Список типов задач.</returns>
    public async Task<IEnumerable<TaskTypeEntity>> GetTaskTypesAsync()
    {
        try
        {
            var result = await _projectManagmentRepository.GetTaskTypesAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ProjectTagEntity>> GetProjectTagsAsync()
    {
        try
        {
            var result = await _projectManagmentRepository.GetProjectTagsAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TaskStatusOutput>> GetTaskStatusesAsync(long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            // TODO: Этот код дублируется в этом сервисе. Вынести в приватный метод и кортежем вернуть нужные данные.
            // Получаем все статусы, которые входят в шаблон текущего проекта.
            // Получаем настройки проекта.
            var projectSettings = await _projectSettingsConfigRepository.GetProjectSpaceSettingsByProjectIdAsync(
                projectId);
            var projectSettingsItems = projectSettings?.ToList();

            if (projectSettingsItems is null || !projectSettingsItems.Any())
            {
                throw new InvalidOperationException("Ошибка получения настроек проекта. " +
                                                    $"ProjectId: {projectId}.");
            }

            var template = projectSettingsItems.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_TEMPLATE_ID));
            var templateId = Convert.ToInt32(template!.ParamValue);

            var statusIds = (await _projectManagmentTemplateRepository.GetTemplateStatusIdsAsync(templateId))
                ?.ToList();

            if (statusIds is null || !statusIds.Any())
            {
                throw new InvalidOperationException("Не удалось получить статусы для выбора в задаче." +
                                                    $"ProjectId: {projectId}");
            }

            var items = await _projectManagmentTemplateRepository.GetTaskTemplateStatusesAsync(statusIds);
            var result = _mapper.Map<IEnumerable<TaskStatusOutput>>(items).ToList();
        
            // Проставляем Id шаблона статусам.
            await SetProjectManagmentTemplateIdsAsync(result);

            var removedTaskStatus = new List<TaskStatusOutput>();

            foreach (var ts in result)
            {
                // Если шаблон не совпадает с шаблоном текущего проекта, то такой статус не нужен.
                if (ts.TemplateId != templateId)
                {
                    removedTaskStatus.Add(ts);
                }
            }

            // Если есть, что удалять, то удаляем статусы, которые не входят в шаблон проекта.
            if (removedTaskStatus.Any())
            {
                result.RemoveAll(x => removedTaskStatus.Select(y => y.StatusId).Contains(x.StatusId));
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает пользователей, которые могут быть выбраны в качестве исполнителя задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список пользователей.</returns>
    public async Task<IEnumerable<ProfileInfoEntity>> GetSelectTaskExecutorsAsync(long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var teamId = await _projectRepository.GetProjectTeamIdByProjectIdAsync(projectId);

            if (teamId <= 0)
            {
                throw new InvalidOperationException($"Не удалось получить команду проекта. ProjectId: {projectId}");
            }

            var teamMemberIds = await _projectRepository.GetProjectTeamMemberIdsAsync(teamId);
            var teamMemberIdsItems = teamMemberIds?.ToList();

            // Если результат null или нету пользователей или есть 1 пользователь и если этот пользователь тот,
            // кто создает сейчас задачу, то список будет пустым.
            // Потому что текущий пользователь может назначить себя исполнителем задачи через кнопку "Назначить меня".
            if (teamMemberIdsItems is null 
                || !teamMemberIdsItems.Any() 
                || (teamMemberIdsItems.Count == 1 && teamMemberIdsItems.First() == userId))
            {
                return Enumerable.Empty<ProfileInfoEntity>();
            }

            var executors = await _userRepository.GetProfileInfoByUserIdsAsync(teamMemberIdsItems);
            var executorsItems = executors?.ToList();

            if (executorsItems is null || !executorsItems.Any())
            {
                return Enumerable.Empty<ProfileInfoEntity>();
            }

            return executorsItems;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task CreateProjectTagAsync(string tagName, string tagDescription, long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var tagSysName = await _reversoService.Value.TranslateTextRussianToEnglishAsync(tagName,
                TranslateLangTypeEnum.Russian, TranslateLangTypeEnum.English);

            // Разбиваем строку на пробелы и приводим каждое слово к PascalCase и соединяем снова в строку.
            tagSysName = string.Join("", tagSysName.Split(" ").Select(x => x.ToPascalCase()));

            var maxUserTagPosition = await _projectManagmentRepository.GetLastPositionUserTaskTagAsync(userId);
            
            // TODO: Для чего вообще использовать класс сущности?
            // TODO: С Dapper не нужно все это.
            // TODO: Использовать просто классы DTO для этого, и факторки эти не нужны будут.
            var userTag = CreateUserTaskTagFactory.CreateProjectTag(tagName, tagDescription, tagSysName,
                    ++maxUserTagPosition, projectId);
            await _projectManagmentRepository.CreateProjectTaskTagAsync(userTag);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectManagmentTaskStatusTemplateEntity>> GetSelectableTaskStatusesAsync(
        long projectId, int templateId)
    {
        try
        {
            var statusIds = await _projectManagmentTemplateRepository.GetTemplateStatusIdsAsync(
                templateId);
            var statusIdsItems = statusIds?.ToList();

            if (statusIdsItems is null || !statusIdsItems.Any())
            {
                throw new InvalidOperationException("Не удалось получить статусы шаблона." +
                                                    $"ProjectId: {projectId}. " +
                                                    $"TemplateId: {templateId}.");
            }

            var result = (await _projectManagmentTemplateRepository.GetTaskTemplateStatusesAsync(statusIdsItems))
                .ToList();

            // TODO: Лучше сразу получить лишь нужные статусы в методе GetTaskTemplateStatusesAsync выше
            // TODO: сделав отдельным методом на это. Пока переиспользуем, но в будущем отрефачить.
            // Удаляем все лишние статусы, кроме базовых для всех шаблонов проекта.
            result.RemoveAll(x => !_associationStatusSysNames.Contains(x.StatusSysName));

            // Если есть оба системных названия, то оставим одно. InWork имеет приоритет.
            if (result.Find(x => x.StatusSysName.Equals("InWork")) is not null
                && result.Find(x => x.StatusSysName.Equals("InDevelopment")) is not null)
            {
                var removedDevelopment = result.Find(x => x.StatusSysName.Equals("InDevelopment"));
                result.Remove(removedDevelopment);
            }

            // Если несколько системных названий Completed, то оставим одно.
            if (result.Count(x => x.StatusSysName.Equals("Completed")) > 1)
            {
                result = result.DistinctBy(d => d.StatusSysName).ToList();
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task CreateUserTaskStatusTemplateAsync(string associationSysName, string statusName,
        string statusDescription, long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var associationStatus = await _projectManagmentTemplateRepository.GetProjectManagementStatusBySysNameAsync(
                associationSysName);

            var associationStatusSysName = associationStatus.StatusSysName;

            // Если системное название статуса недопустимо, то кидаем исключение.
            if (!_associationStatusSysNames.Contains(associationStatusSysName))
            {
                throw new InvalidOperationException("Системное название статуса не определенно как базовый. " +
                                                    "Проверьте маппинг статуса в таблице TaskStatuses" +
                                                    $" StatusSysName: {associationStatusSysName}. " +
                                                    $"AssociationStatusSysName: {associationStatusSysName}. " +
                                                    $"StatusName: {statusName}. " +
                                                    $"ProjectId: {projectId}");
            }

            var lastUserPosition = await _projectManagmentTemplateRepository.GetLastPositionUserStatusTemplateAsync(
                userId);
            
            var statusSysName = await _reversoService.Value.TranslateTextRussianToEnglishAsync(statusName,
                TranslateLangTypeEnum.Russian, TranslateLangTypeEnum.English);

            // Разбиваем строку на пробелы и приводим каждое слово к PascalCase и соединяем снова в строку.
            // Это нужно, если пользователь ввел название с пробелами и тд.
            statusSysName = string.Join("", statusSysName.Split(" ").Select(x => x.ToPascalCase()));

            // TODO: Для чего вообще использовать класс сущности?
            // TODO: С Dapper не нужно все это.
            // TODO: Использовать просто классы DTO для этого, и факторки эти не нужны будут.
            var addedCustomUserStatus = CreateTaskStatusFactory.CreateUserStatuseTemplate(statusName, statusSysName,
                ++lastUserPosition, userId, statusDescription);

            // Создаем кастомный статус пользователя.
            // Кастомный статус добавляется в шаблон пользователя (расширение шаблона), по которому управляется проект.
            // Исходный шаблон проекта не изменяется и его статусы тоже.
            var addedStatusId = await _projectManagmentTemplateRepository
                .CreateProjectManagmentTaskStatusTemplateAsync(addedCustomUserStatus);

            if (addedStatusId <= 0)
            {
                throw new InvalidOperationException(
                    "Невалидный Id кастомного статуса. " +
                    "Возможно, добавление не произошло. " +
                    "Нужно првоерить таблицу кастомных статусов шаблонов пользователей. " +
                    $"ProjectId: {projectId}. " +
                    $"Id кастомного статуса был: {addedStatusId}");
            }
            
            // TODO: Изменить на получение шаблона из репозитория конфигов настроек проектов.
            // Получаем шаблон проекта, если он был выбран.
            var projectTemplateId = await _projectManagmentTemplateRepository.GetProjectTemplateIdAsync(projectId);

            if (!projectTemplateId.HasValue || projectTemplateId.Value <= 0)
            {
                throw new InvalidOperationException($"Не удалось получить шаблон проекта. ProjectId: {projectId}");
            }
            
            // Добавляем этот новый кастомный статус в маппинг статусов шаблонов.
            await _projectManagmentTemplateRepository.CreateProjectManagmentTaskStatusIntermediateTemplateAsync(
                addedStatusId, projectTemplateId.Value);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AvailableTaskStatusTransitionOutput>> GetAvailableTaskStatusTransitionsAsync(
        long projectId, string projectTaskId, string taskDetailType)
    {
        try
        {
            var detailType = Enum.Parse<TaskDetailTypeEnum>(taskDetailType);
            var onlyProjectTaskId = projectTaskId.GetProjectTaskIdFromPrefixLink();
            bool ifProjectHavingTask;
            long currentTaskStatusId = 0;
            var transitionType = TransitionTypeEnum.None;

            if (detailType is TaskDetailTypeEnum.Task or TaskDetailTypeEnum.Error)
            {
                ifProjectHavingTask = await _projectManagmentRepository.IfProjectHavingProjectTaskIdAsync(projectId,
                    onlyProjectTaskId);

                // Если задача не принадлежит проекту.
                if (!ifProjectHavingTask)
                {
                    throw new InvalidOperationException("Задача не принадлежит проекту. " +
                                                        $"ProjectId: {projectId}. " +
                                                        $"ProjectTaskId: {projectTaskId}");
                }
                
                // Получаем текущий статус задачи.
                currentTaskStatusId = await _projectManagmentRepository
                    .GetProjectTaskStatusIdByProjectIdProjectTaskIdAsync(projectId, onlyProjectTaskId);

                if (currentTaskStatusId <= 0)
                {
                    throw new InvalidOperationException("Не удалось получить текущий статус задачи. " +
                                                        $"ProjectId: {projectId}. " +
                                                        $"ProjectTaskId: {projectTaskId}");
                }

                transitionType = TransitionTypeEnum.Task;
            }

            if (detailType == TaskDetailTypeEnum.Epic)
            {
                ifProjectHavingTask = await _projectManagmentRepository.IfProjectHavingEpicIdAsync(projectId,
                    onlyProjectTaskId);

                // Если эпик не принадлежит проекту.
                if (!ifProjectHavingTask)
                {
                    throw new InvalidOperationException("Эпик не принадлежит проекту. " +
                                                        $"ProjectId: {projectId}. " +
                                                        $"ProjectEpicId: {projectTaskId}");
                }
                
                // Получаем текущий статус эпика.
                currentTaskStatusId = await _projectManagmentRepository
                    .GetProjectEpicStatusIdByProjectIdEpicIdIdAsync(projectId, onlyProjectTaskId);

                if (currentTaskStatusId <= 0)
                {
                    throw new InvalidOperationException("Не удалось получить текущий статус эпика. " +
                                                        $"ProjectId: {projectId}. " +
                                                        $"ProjectEpicId: {projectTaskId}");
                }
                
                transitionType = TransitionTypeEnum.Epic;
            }
            
            if (detailType == TaskDetailTypeEnum.History)
            {
                ifProjectHavingTask = await _projectManagmentRepository.IfProjectHavingProjectUserStoryIdAsync(
                    projectId, onlyProjectTaskId);

                // Если история не принадлежит проекту.
                if (!ifProjectHavingTask)
                {
                    throw new InvalidOperationException("История не принадлежит проекту. " +
                                                        $"ProjectId: {projectId}. " +
                                                        $"ProjectEpicId: {projectTaskId}");
                }
                
                // Получаем текущий статус истории.
                currentTaskStatusId = await _projectManagmentRepository
                    .GetProjectUserStoryStatusIdByUserStoryIdAsync(projectId, onlyProjectTaskId);

                if (currentTaskStatusId <= 0)
                {
                    throw new InvalidOperationException("Не удалось получить текущий статус истории. " +
                                                        $"ProjectId: {projectId}. " +
                                                        $"ProjectEpicId: {projectTaskId}");
                }
                
                transitionType = TransitionTypeEnum.History;
            }

            // Получаем все переходы из промежуточной таблицы отталкиваясь от текущего статуса истории.
            var statusIds = (await _projectManagmentRepository
                    .GetProjectManagementTransitionIntermediateTemplatesAsync(currentTaskStatusId, transitionType))
                ?.AsList();

            if (statusIds is null || !statusIds.Any())
            {
                throw new InvalidOperationException(
                    $"Не удалось получить доступные переходы для статуса {currentTaskStatusId}. " +
                    $"ProjectId: {projectId}. " +
                    $"ProjectTaskId: {projectTaskId}.");
            }

            // Получаем все статусы по переходам.
            var transitionStatuses = (await _projectManagmentRepository.GetTaskStatusIntermediateTemplatesAsync(
                statusIds))?.AsList();

            if (transitionStatuses is null || !transitionStatuses.Any())
            {
                throw new InvalidOperationException(
                    $"Не удалось получить статусы переходов: {JsonConvert.SerializeObject(transitionStatuses)}. " +
                    $"Id статусов были: {JsonConvert.SerializeObject(statusIds)} " +
                    $"ProjectId: {projectId}. " +
                    $"ProjectTaskId: {projectTaskId}. " +
                    $"Id текущего статуса задачи: {currentTaskStatusId}.");
            }
            
            // Получаем названия статусов у таких переходов.
            // Опираемся на признаки кастомности промежуточных таблиц переходов и статусов
            // IsCustomTransition и IsCustomStatus.
            var commonStatuses = await _projectManagmentRepository.GetTaskStatusTemplatesAsync();
            
            // Если нету общих статусов, но среди переходов был минимум 1 общий,
            // то это ошибка и опасно продолжать дальше. Это может нарушить целостность переходов.
            // TODO: Почему это может нарушить целостность переходов? Точно ли нужна эта проверка?
            if (commonStatuses is null)
            {
                throw new InvalidOperationException(
                    "Не удалось получить кастомные статусы пользователя," +
                    " хотя был минимум 1 кастомный статус среди: " +
                    $"{JsonConvert.SerializeObject(transitionStatuses)}.");
            }
            
            var userStatuses = await _projectManagmentRepository.GetUserTaskStatusTemplatesAsync();
            
            if (userStatuses is null)
            {
                throw new InvalidOperationException(
                    "Не удалось получить кастомные статусы пользователя," +
                    " хотя был минимум 1 кастомный статус среди: " +
                    $"{JsonConvert.SerializeObject(transitionStatuses)}.");
            }
            
            // TODO: Этот код дублируется в этом сервисе. Вынести в приватный метод и кортежем вернуть нужные данные.
            // Получаем настройки проекта.
            var projectSettings = await _projectSettingsConfigRepository.GetProjectSpaceSettingsByProjectIdAsync(
                projectId);
            var projectSettingsItems = projectSettings?.AsList();

            if (projectSettingsItems is null || !projectSettingsItems.Any())
            {
                throw new InvalidOperationException("Ошибка получения настроек проекта. " +
                                                    $"ProjectId: {projectId}.");
            }

            var template = projectSettingsItems.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_TEMPLATE_ID));
            var templateId = Convert.ToInt32(template!.ParamValue);

            // Получаем все Id статусов, которые входят в шаблон текущего проекта.
            // Получаем все статусы, которые входят в шаблон текущего проекта.
            var templateStatusIds = (await _projectManagmentTemplateRepository.GetTemplateStatusIdsAsync(templateId))
                ?.AsList();

            if (templateStatusIds is null || !templateStatusIds.Any())
            {
                throw new InvalidOperationException($"Не удалось получить статусы шаблона проекта: {templateId}.");
            }
            
            // Получаем сами статусы, которые входят в шаблон текущего проекта.
            var templateStatuses = (await _projectManagmentTemplateRepository
                .GetTaskTemplateStatusesAsync(templateStatusIds))?.ToDictionary(k => k.StatusId, v => v);

            if (templateStatuses is null || !templateStatuses.Any())
            {
                throw new InvalidOperationException(
                    $"Не удалось получить статусы шаблона проекта: {projectId}. " +
                    $"Id статусов были: {JsonConvert.SerializeObject(templateStatusIds)}.");
            }

            var result = new List<AvailableTaskStatusTransitionOutput>();
            
            foreach (var ts in transitionStatuses)
            {
                // Если шаблон не совпадает с шаблоном текущего проекта, то такой статус не нужен.
                if (ts.TemplateId != templateId)
                {
                    continue;
                }
                
                var statusId = ts.StatusId;
                
                if (statusId <= 0)
                {
                    throw new InvalidOperationException($"Невалидный статус задачи: {statusId}. " +
                                                        $"Id задачи: {projectTaskId}. " +
                                                        $"Id проекта: {projectId}.");
                }

                // Если статус не входит в шаблон проекта, то исключаем его.
                if (!templateStatuses.ContainsKey(statusId))
                {
                    continue;
                }

                // Если кастомный статус, то получать будем из таблицы кастомных статусов.
                if (ts.IsCustomStatus)
                {
                    if (!userStatuses.ContainsKey(statusId))
                    {
                        throw new InvalidOperationException(
                            $"Статуса {statusId} нет среди статусов пользователей. " +
                            "Возможно, стоит проверить среди общих статусов," +
                            " а не среди кастомных либо статус некорректен. " +
                            $"Кастомные статусы были: {JsonConvert.SerializeObject(userStatuses)}" +
                            $". Среди них не было найдено статуса: {statusId}.");
                    }
                    
                    var userStatus = userStatuses.TryGet(statusId);
                    
                    result.Add(new AvailableTaskStatusTransitionOutput
                    {
                        StatusName = userStatus.StatusName,
                        StatusId = statusId,
                        TaskStatusId = ts.TaskStatusId
                    });
                }

                // Иначе из таблицы общих статусов.
                if (!commonStatuses.ContainsKey(statusId))
                {
                    throw new InvalidOperationException(
                        $"Статуса {statusId} нет среди общих статусов. " +
                        "Возможно, стоит проверить среди кастомных статусов пользователей," +
                        " а не среди общих либо статус некорректен. " +
                        $"Общие статусы были: {JsonConvert.SerializeObject(commonStatuses)}" +
                        $". Среди них не было найдено статуса: {statusId}.");
                }
                
                var commonStatuse = commonStatuses.TryGet(statusId);
                    
                result.Add(new AvailableTaskStatusTransitionOutput
                {
                    StatusName = commonStatuse.StatusName,
                    StatusId = statusId,
                    TaskStatusId = ts.TaskStatusId
                });
            }

            // Добавляем текущий статус в доступный переход.
            var currentTaskStatus = await _projectManagmentRepository
                .GetTaskStatusByTaskStatusIdAsync(currentTaskStatusId, templateId);

            // Не добавляем текущий статус для этих типов задач, так как у них есть свои и они не кастомные.
            if (!new[] { TransitionTypeEnum.Epic, TransitionTypeEnum.History, TransitionTypeEnum.Sprint }.Contains(
                    transitionType))
            {
                result.Add(new AvailableTaskStatusTransitionOutput
                {
                    StatusName = currentTaskStatus.StatusName,
                    StatusId = currentTaskStatus.StatusId,
                    TaskStatusId = currentTaskStatus.TaskStatusId
                });
            }

            // Дополняем статусами, в зависимости от типа задачи.
            // Если нужно получить доступные статусы (переходы) для эпика.
            if (transitionType == TransitionTypeEnum.Epic)
            {
                // TODO: Если в будущем будет функционал для создания кастомных статусов эпика пользователем,
                // TODO: то придется заводить поле TaskStatusId в таблице статусов эпиков и тогда его тут получать уже.
                // Сейчас StatusId и TaskStatusId у эпиков одинаковые будут, так как нет отдельного поля под TaskStatusId у них,
                // потому что создание кастомных статусов для эпика пока не предполагается в системе.
                var epicStatuses = await _projectManagmentRepository.GetEpicStatusesAsync();
                result.AddRange(epicStatuses.Select(x => new AvailableTaskStatusTransitionOutput
                {
                    StatusName = x.StatusName,
                    StatusId = x.StatusId,
                    TaskStatusId = x.StatusId
                }));
            }
            
            // Дополняем статусами, в зависимости от типа задачи.
            // Если нужно получить доступные статусы (переходы) для истории.
            if (transitionType == TransitionTypeEnum.History)
            {
                // TODO: Если в будущем будет функционал для создания кастомных статусов эпика пользователем,
                // TODO: то придется заводить поле TaskStatusId в таблице статусов эпиков и тогда его тут получать уже.
                // Сейчас StatusId и TaskStatusId у историй одинаковые будут, так как нет отдельного поля под TaskStatusId у них,
                // потому что создание кастомных статусов для историй пока не предполагается в системе.
                var storyStatuses = await _projectManagmentRepository.GetUserStoryStatusesAsync();
                result.AddRange(storyStatuses.Select(x => new AvailableTaskStatusTransitionOutput
                {
                    StatusName = x.StatusName,
                    StatusId = x.StatusId,
                    TaskStatusId = x.StatusId
                }));
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task ChangeTaskStatusAsync(long projectId, string changeStatusId, string taskId,
        string taskDetailType)
    {
        var detailType = Enum.Parse<TaskDetailTypeEnum>(taskDetailType);
        var onlyTaskId = taskId.GetProjectTaskIdFromPrefixLink();
        
        switch (detailType)
        {
            case TaskDetailTypeEnum.Task or TaskDetailTypeEnum.Error:
                await _projectManagmentRepository.ChangeTaskStatusAsync(projectId,
                    changeStatusId.GetProjectTaskIdFromPrefixLink(), onlyTaskId);
                break;

            case TaskDetailTypeEnum.Epic:
                await _projectManagmentRepository.ChangeEpicStatusAsync(projectId,
                    changeStatusId.GetProjectTaskIdFromPrefixLink(), onlyTaskId);
                break;
            
            case TaskDetailTypeEnum.History:
                await _projectManagmentRepository.ChangeStoryStatusAsync(projectId,
                    changeStatusId.GetProjectTaskIdFromPrefixLink(), onlyTaskId);
                break;
                
            case TaskDetailTypeEnum.Sprint:
                await _projectManagmentRepository.ChangeSprintStatusAsync(projectId,
                    changeStatusId.GetProjectTaskIdFromPrefixLink(), onlyTaskId);
                break;
        }
    }

    /// <inheritdoc />
    public async Task UpdateTaskDetailsAsync(long projectId, string taskId, string changedTaskDetails, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            await _projectManagmentRepository.UpdateTaskDetailsAsync(projectId, taskId.GetProjectTaskIdFromPrefixLink(),
                changedTaskDetails);
            
            // TODO: Тут добавить запись активности пользователя по userId.
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateTaskNameAsync(long projectId, string taskId, string changedTaskName, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            await _projectManagmentRepository.UpdateTaskNameAsync(projectId, taskId.GetProjectTaskIdFromPrefixLink(),
                changedTaskName);
            
            // TODO: Тут добавить запись активности пользователя по userId.
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task AttachTaskTagAsync(int tagId, string projectTaskId, long projectId, string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        await _projectManagmentRepository.AttachTaskTagAsync(tagId, projectTaskId.GetProjectTaskIdFromPrefixLink(),
            projectId);

        // TODO: Тут добавить запись активности пользователя по userId.
    }

    /// <inheritdoc />
    public async Task DetachTaskTagAsync(int tagId, string projectTaskId, long projectId, string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        await _projectManagmentRepository.DetachTaskTagAsync(tagId, projectTaskId.GetProjectTaskIdFromPrefixLink(),
            projectId);

        // TODO: Тут добавить запись активности пользователя по userId.
    }

    /// <inheritdoc />
    public async Task AttachTaskWatcherAsync(long watcherId, string projectTaskId, long projectId, string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        await _projectManagmentRepository.AttachTaskWatcherAsync(watcherId,
            projectTaskId.GetProjectTaskIdFromPrefixLink(), projectId);

        // TODO: Тут добавить запись активности пользователя по userId.
    }

    /// <inheritdoc />
    public async Task DetachTaskWatcherAsync(long watcherId, string projectTaskId, long projectId, string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        await _projectManagmentRepository.DetachTaskWatcherAsync(watcherId,
            projectTaskId.GetProjectTaskIdFromPrefixLink(), projectId);

        // TODO: Тут добавить запись активности пользователя по userId.
    }

    /// <inheritdoc />
    public async Task UpdateTaskExecutorAsync(long executorId, string projectTaskId, long projectId, string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        await _projectManagmentRepository.UpdateTaskExecutorAsync(executorId,
            projectTaskId.GetProjectTaskIdFromPrefixLink(), projectId);

        // TODO: Тут добавить запись активности пользователя по userId.
    }

    /// <inheritdoc />
    public async Task UpdateTaskPriorityAsync(int priorityId, string projectTaskId, long projectId, string account)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        await _projectManagmentRepository.UpdateTaskPriorityAsync(priorityId,
            projectTaskId.GetProjectTaskIdFromPrefixLink(), projectId);
        
        // TODO: Тут добавить запись активности пользователя по userId.
    }

    /// <inheritdoc />
    public async Task CreateTaskLinkAsync(TaskLinkInput taskLinkInput, string account)
    {
        try
        {
            // Убираем префикс оставляя только Id задач.
            var taskFromLink = taskLinkInput.TaskFromLink.GetProjectTaskIdFromPrefixLink();
            var taskToLink = taskLinkInput.TaskToLink.GetProjectTaskIdFromPrefixLink();
            var projectId = taskLinkInput.ProjectId;
            
            // Валидируем типы связи.
            if (!new[] { LinkTypeEnum.Link, LinkTypeEnum.Child, LinkTypeEnum.Parent, LinkTypeEnum.Depend }.Contains(
                    taskLinkInput.LinkType))
            {
                if (taskFromLink <= 0 || taskToLink <= 0 || projectId <= 0)
                {
                    throw new InvalidOperationException(
                        $"Невалидные входные данные для создания типа связи {taskLinkInput.LinkType}. " +
                        $"TaskFromLink: {taskLinkInput.TaskFromLink}. " +
                        $"TaskToLink: {taskLinkInput.TaskToLink}. " +
                        $"ProjectId: {projectId}. " +
                        $"Account: {account}");
                }
            }

            if (taskLinkInput.LinkType == LinkTypeEnum.Parent)
            {
                // Проставляем родителя.
                taskLinkInput.ParentId = taskLinkInput.TaskToLink;
                
                if (string.IsNullOrWhiteSpace(taskLinkInput.ParentId))
                {
                    throw new InvalidOperationException(
                        "Id родителя невалидный." +
                        $"ParentId: {taskLinkInput.ParentId}. " +
                        $"TaskFromLink: {taskLinkInput.TaskFromLink}. " +
                        $"TaskToLink: {taskLinkInput.TaskToLink}. " +
                        $"ProjectId: {projectId}. " +
                        $"Account: {account}");
                }
            }
            
            if (taskLinkInput.LinkType == LinkTypeEnum.Child)
            {
                // Проставляем дочку.
                taskLinkInput.ChildId = taskLinkInput.TaskToLink;
                
                if (string.IsNullOrWhiteSpace(taskLinkInput.ChildId))
                {
                    throw new InvalidOperationException(
                        "Id дочки невалидный." +
                        $"ChildId: {taskLinkInput.ChildId}. " +
                        $"TaskFromLink: {taskLinkInput.TaskFromLink}. " +
                        $"TaskToLink: {taskLinkInput.TaskToLink}. " +
                        $"ProjectId: {projectId}. " +
                        $"Account: {account}");
                }
            }
            
            if (taskLinkInput.LinkType == LinkTypeEnum.Depend)
            {
                // Проставляем от какой задачи зависит текущая.
                taskLinkInput.DependId = taskLinkInput.TaskToLink;
                
                if (string.IsNullOrWhiteSpace(taskLinkInput.DependId))
                {
                    throw new InvalidOperationException(
                        "Id блокирующей задачи невалидный." +
                        $"DependId: {taskLinkInput.DependId}. " +
                        $"TaskFromLink: {taskLinkInput.TaskFromLink}. " +
                        $"TaskToLink: {taskLinkInput.TaskToLink}. " +
                        $"ProjectId: {projectId}. " +
                        $"Account: {account}");
                }
            }
            
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
        
            var currentTask = await _projectManagmentRepository.GetTaskDetailsByTaskIdAsync(taskFromLink, projectId);
                
            if (currentTask is null)
            {
                throw new InvalidOperationException(
                    "Не удалось получить текущую задачу." +
                    $"ProjectId: {projectId}. " +
                    $"ProjectTaskId: {taskLinkInput.TaskFromLink}.");
            }

            // Создаем связь в БД.
            // TaskFromLink - Id задачи в рамках проекта становится Id задачи.
            await _projectManagmentRepository.CreateTaskLinkAsync(currentTask.TaskId,
                taskLinkInput.TaskToLink.GetProjectTaskIdFromPrefixLink(),
                taskLinkInput.LinkType,
                projectId,
                !string.IsNullOrWhiteSpace(taskLinkInput.ChildId)
                    ? taskLinkInput.ChildId.GetProjectTaskIdFromPrefixLink()
                    : null,
                !string.IsNullOrWhiteSpace(taskLinkInput.ParentId)
                    ? taskLinkInput.ParentId.GetProjectTaskIdFromPrefixLink()
                    : null,
                !string.IsNullOrWhiteSpace(taskLinkInput.DependId)
                    ? taskLinkInput.DependId.GetProjectTaskIdFromPrefixLink()
                    : null);

            // TODO: Тут добавить запись активности пользователя по userId.
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkDefaultAsync(long projectId, string projectTaskId,
        LinkTypeEnum linkType)
    {
        try
        {
            var currentTask = await _projectManagmentRepository.GetTaskDetailsByTaskIdAsync(
                    projectTaskId.GetProjectTaskIdFromPrefixLink(), projectId);
                
            if (currentTask is null)
            {
                throw new InvalidOperationException(
                    "Не удалось получить текущую задачу." +
                    $"ProjectId: {projectId}. ProjectTaskId: {projectTaskId}.");
            }

            var links = (await _projectManagmentRepository.GetTaskLinksByProjectIdProjectTaskIdAsync(projectId,
                currentTask.TaskId, linkType))
                ?.AsList();

            if (links is null || !links.Any())
            {
                return Enumerable.Empty<GetTaskLinkOutput>();
            }

            // У обычных связей нет родителя и ребенка.
            var linkIds = links
                .Where(x => x.ParentId is null && x.ChildId is null)
                .Select(x => x.ToTaskId!.Value);
            
            var tasks = (await _projectManagmentRepository.GetProjectTaskByProjectIdTaskIdsAsync(projectId, linkIds))
                ?.AsList();

            if (tasks is null || !tasks.Any())
            {
                throw new InvalidOperationException(
                    "Не удалось получить связи задачи по Id проекта и Id задачи в рамках проекта." +
                    $"ProjectId: {projectId}. ProjectTaskId: {projectTaskId}.");
            }

            var result = await ModifyTaskLinkResultAsync(tasks);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AvailableTaskLinkOutput>> GetAvailableTaskLinkAsync(long projectId,
        LinkTypeEnum linkType)
    {
        try
        {
            var result = (await _projectManagmentRepository.GetAvailableTaskLinkAsync(projectId, linkType)).AsList();
            
            // Получаем имена исполнителей задач.
            var executorIds = result.Where(x => x.ExecutorId > 0).Select(x => x.ExecutorId);
            var executors = await _userRepository.GetExecutorNamesByExecutorIdsAsync(executorIds);

            if (executors.Count == 0)
            {
                throw new InvalidOperationException("Не удалось получить исполнителей задач.");
            }
        
            var statusIds = result.Select(x => x.TaskStatusId);
            var statuseNames = (await _projectManagmentTemplateRepository.GetTaskTemplateStatusesAsync(statusIds))
                .ToList();

            // Наполняем выходные данные задач.
            foreach (var t in result)
            {
                t.ExecutorName = executors.TryGet(t.ExecutorId).FullName;
                t.TaskStatusName = statuseNames.Find(x => x.StatusId == t.TaskStatusId)?.StatusName;
                t.LinkType = linkType;
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkParentAsync(long projectId, string projectTaskId,
        LinkTypeEnum linkType)
    {
        try
        {
            var currentTask = await _projectManagmentRepository.GetTaskDetailsByTaskIdAsync(
                    projectTaskId.GetProjectTaskIdFromPrefixLink(), projectId);
                
            if (currentTask is null)
            {
                throw new InvalidOperationException(
                    "Не удалось получить текущую задачу." +
                    $"ProjectId: {projectId}. ProjectTaskId: {projectTaskId}.");
            }

            var links = (await _projectManagmentRepository.GetTaskLinksByProjectIdProjectTaskIdAsync(projectId,
                    currentTask.TaskId, linkType))
                ?.AsList();

            if (links is null || !links.Any())
            {
                return Enumerable.Empty<GetTaskLinkOutput>();
            }
            
            var linkIds = links
                .Where(x => x.ParentId is not null)
                .Select(x => x.ParentId!.Value);
            
            var tasks = (await _projectManagmentRepository.GetProjectTaskByProjectIdTaskIdsAsync(projectId, linkIds))
                ?.AsList();

            if (tasks is null || !tasks.Any())
            {
                return Enumerable.Empty<GetTaskLinkOutput>();
            }

            var result = await ModifyTaskLinkResultAsync(tasks);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkChildAsync(long projectId, string projectTaskId,
        LinkTypeEnum linkType)
    {
        try
        {
            var currentTask = await _projectManagmentRepository.GetTaskDetailsByTaskIdAsync(
                    projectTaskId.GetProjectTaskIdFromPrefixLink(), projectId);
                
            if (currentTask is null)
            {
                throw new InvalidOperationException(
                    "Не удалось получить текущую задачу." +
                    $"ProjectId: {projectId}. ProjectTaskId: {projectTaskId}.");
            }

            var links = (await _projectManagmentRepository.GetTaskLinksByProjectIdProjectTaskIdAsync(projectId,
                    currentTask.TaskId, linkType))
                ?.AsList();

            if (links is null || !links.Any())
            {
                return Enumerable.Empty<GetTaskLinkOutput>();
            }
            
            var linkIds = links
                .Where(x => x.ChildId is not null)
                .Select(x => x.ChildId!.Value);
            
            var tasks = (await _projectManagmentRepository.GetProjectTaskByProjectIdTaskIdsAsync(projectId, linkIds))
                ?.AsList();

            if (tasks is null || !tasks.Any())
            {
                return Enumerable.Empty<GetTaskLinkOutput>();
            }

            var result = await ModifyTaskLinkResultAsync(tasks);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkDependAsync(long projectId, string projectTaskId,
        LinkTypeEnum linkType)
    {
        try
        {
            var currentTask = await _projectManagmentRepository.GetTaskDetailsByTaskIdAsync(
                    projectTaskId.GetProjectTaskIdFromPrefixLink(), projectId);
                
            if (currentTask is null)
            {
                throw new InvalidOperationException(
                    "Не удалось получить текущую задачу." +
                    $"ProjectId: {projectId}. ProjectTaskId: {projectTaskId}.");
            }

            var links = (await _projectManagmentRepository.GetTaskLinksByProjectIdProjectTaskIdAsync(projectId,
                    currentTask.TaskId, linkType))
                ?.AsList();

            if (links is null || !links.Any())
            {
                return Enumerable.Empty<GetTaskLinkOutput>();
            }
            
            // Получаем задачи, которые блокируют текущую. Текущая зависит от них.
            var linkIds = links
                .Where(x => x.IsBlocked)
                .Select(x => x.FromTaskId!.Value);
            
            var tasks = (await _projectManagmentRepository.GetProjectTaskByProjectIdTaskIdsAsync(projectId, linkIds))
                ?.AsList();

            if (tasks is null || !tasks.Any())
            {
                return Enumerable.Empty<GetTaskLinkOutput>();
            }

            var result = await ModifyTaskLinkResultAsync(tasks);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkBlockedAsync(long projectId, string projectTaskId,
        LinkTypeEnum linkType)
    {
        try
        {
            var currentTask = await _projectManagmentRepository.GetTaskDetailsByTaskIdAsync(
                    projectTaskId.GetProjectTaskIdFromPrefixLink(), projectId);
                
            if (currentTask is null)
            {
                throw new InvalidOperationException(
                    "Не удалось получить текущую задачу." +
                    $"ProjectId: {projectId}. ProjectTaskId: {projectTaskId}.");
            }

            var links = (await _projectManagmentRepository.GetTaskLinksByProjectIdProjectTaskIdAsync(projectId,
                    currentTask.TaskId, linkType))
                ?.AsList();

            if (links is null || !links.Any())
            {
                return Enumerable.Empty<GetTaskLinkOutput>();
            }
            
            // Получаем задачи, которые текущая задача блокирует.
            var linkIds = links
                .Where(x => x.FromTaskId == currentTask.TaskId && x.IsBlocked)
                .Select(x => x.BlockedTaskId!.Value);
            
            var tasks = (await _projectManagmentRepository.GetProjectTaskByProjectIdTaskIdsAsync(projectId, linkIds))
                ?.AsList();

            if (tasks is null || !tasks.Any())
            {
                return Enumerable.Empty<GetTaskLinkOutput>();
            }

            var result = await ModifyTaskLinkResultAsync(tasks);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RemoveTaskLinkAsync(LinkTypeEnum linkType, string removedLinkId, string currentTaskId,
        long projectId, string account)
    {
        try
        {
            // Разрываем связь в БД.
            await _projectManagmentRepository.RemoveTaskLinkAsync(linkType,
                removedLinkId.GetProjectTaskIdFromPrefixLink(),
                currentTaskId.GetProjectTaskIdFromPrefixLink(),
                projectId);

            // TODO: Тут добавить запись активности пользователя по userId.
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UploadFilesAsync(IFormFileCollection files, string account, long projectId, string taskId)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            await _fileManagerService.Value.UploadFilesAsync(files, projectId, taskId.GetProjectTaskIdFromPrefixLink());

            // TODO: Для чего вообще использовать класс сущности?
            // TODO: С Dapper не нужно все это.
            // TODO: Использовать просто классы DTO для этого, и факторки эти не нужны будут.
            var projectTaskFiles = CreateProjectDocumentsFactory.CreateProjectDocuments(files, projectId,
                taskId.GetProjectTaskIdFromPrefixLink(), userId);
            
            // Сохраняем файлы задачи проекта.
            await _projectManagmentRepository.CreateProjectTaskDocumentsAsync(projectTaskFiles,
                DocumentTypeEnum.ProjectTask);

            // TODO: Тут добавить запись активности пользователя по userId (писать кто добавил файл).
        }
        
        catch (Exception ex)
        {
             _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<FileContentResult> DownloadFileAsync(long documentId, long projectId, string projectTaskId)
    {
        try
        {
            var task = await _projectManagmentRepository.GetTaskDetailsByTaskIdAsync(
                projectTaskId.GetProjectTaskIdFromPrefixLink(), projectId);
            
            if (task is null)
            {
                throw new InvalidOperationException("Не удалось получить задачу. " +
                                                    $"ProjectId: {projectId}. " +
                                                    $"ProjectTaskId: {projectTaskId}.");
            }

            var documentName = await _projectManagmentRepository.GetDocumentNameByDocumentIdAsync(documentId);
            
            var result = await _fileManagerService.Value.DownloadFileAsync(documentName, projectId, task.TaskId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ProjectDocumentEntity>> GetProjectTaskFilesAsync(long projectId,
        string projectTaskId, TaskDetailTypeEnum taskDetailType)
    {
        try
        {
            long taskId = 0;
            long projectTaskIdNumber = projectTaskId.GetProjectTaskIdFromPrefixLink();
            
            if (taskDetailType is TaskDetailTypeEnum.Task or TaskDetailTypeEnum.Error)
            {
                var task = await _projectManagmentRepository.GetTaskDetailsByTaskIdAsync(projectTaskIdNumber,
                    projectId);
            
                if (task is null)
                {
                    throw new InvalidOperationException("Не удалось получить задачу. " +
                                                        $"ProjectId: {projectId}. " +
                                                        $"ProjectTaskId: {projectTaskId}.");
                }

                taskId = task.TaskId;
            }

            if (taskDetailType == TaskDetailTypeEnum.Epic)
            {
                var task = await _projectManagmentRepository.GetEpicDetailsByEpicIdAsync(projectTaskIdNumber,
                    projectId);
            
                if (task is null)
                {
                    throw new InvalidOperationException("Не удалось получить эпик. " +
                                                        $"ProjectId: {projectId}. " +
                                                        $"ProjectTaskId: {projectTaskId}.");
                }
                
                taskId = task.EpicId;
            }

            var result = await _projectManagmentRepository.GetProjectTaskFilesAsync(projectId, taskId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RemoveTaskFileAsync(long documentId, long projectId, string projectTaskId)
    {
        try
        {
            var task = await _projectManagmentRepository.GetTaskDetailsByTaskIdAsync(
                projectTaskId.GetProjectTaskIdFromPrefixLink(), projectId);
            
            if (task is null)
            {
                throw new InvalidOperationException("Не удалось получить задачу. " +
                                                    $"ProjectId: {projectId}. " +
                                                    $"ProjectTaskId: {projectTaskId}.");
            }

            var documentName = await _projectManagmentRepository.GetDocumentNameByDocumentIdAsync(documentId);
            
            // Удаляем файл на сервере.
            await _fileManagerService.Value.RemoveFileAsync(documentName, projectId, task.TaskId);
            
            // Удаляем файл в БД.
            await _projectManagmentRepository.RemoveDocumentAsync(documentId);
            
            // TODO: Тут добавить запись активности пользователя по userId (писать кто удалил файл).
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task FixationProjectViewStrategyAsync(string strategySysName, long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            string shortSysName;

            switch (strategySysName)
            {
                case "Kanban":
                    shortSysName = "kn";
                    break;

                case "Scrum":
                    shortSysName = "sm";
                    break;

                default:
                    throw new InvalidOperationException($"Неизвестный тип стратегии: {strategySysName}.");
            }

            await _projectManagmentRepository.FixationProjectViewStrategyAsync(shortSysName, projectId, userId);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task CreateTaskCommentAsync(string projectTaskId, long projectId, string comment, string account)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                return;
            }
            
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            await _projectManagmentRepository.CreateTaskCommentAsync(projectTaskId.GetProjectTaskIdFromPrefixLink(),
                projectId, comment, userId);
            
            // TODO: Тут добавить запись активности пользователя по userId.
        }
        
        catch (Exception ex)
        {
             _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TaskCommentOutput>> GetTaskCommentsAsync(string projectTaskId, long projectId)
    {
        try
        {
            var items = (await _projectManagmentRepository.GetTaskCommentsAsync(
                projectTaskId.GetProjectTaskIdFromPrefixLink(), projectId))?.AsList();

            if (items is null || !items.Any())
            {
                return Enumerable.Empty<TaskCommentOutput>();
            }

            var result = _mapper.Map<IEnumerable<TaskCommentOutput>>(items);

            return result;
        }
        
        catch (Exception ex)
        {
             _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateTaskCommentAsync(string projectTaskId, long projectId, long commentId, string comment,
        string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            await _projectManagmentRepository.UpdateTaskCommentAsync(projectTaskId.GetProjectTaskIdFromPrefixLink(),
                projectId, commentId, comment, userId);
            
            // TODO: Тут добавить запись активности пользователя по userId.
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteTaskCommentAsync(long commentId, string account)
    {
        try
        {
            await _projectManagmentRepository.DeleteTaskCommentAsync(commentId);
            
            // TODO: Тут добавить запись активности пользователя по userId.
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<FileContentResult> GetUserAvatarFileAsync(long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var documentId = await _projectManagmentRepository.GetUserAvatarDocumentIdByUserIdAsync(userId, projectId);

            // Если у пользователя не был выбран аватар ранее, то подгрузим дефолтный nophoto.
            if (!documentId.HasValue)
            {
                var noPhotoResult = await _fileManagerService.Value.GetUserAvatarFileAsync("nophoto.jpg",
                    projectId, userId, true);

                return noPhotoResult;
            }

            var documentName = await _projectManagmentRepository.GetDocumentNameByDocumentIdAsync(documentId.Value);

            var result = await _fileManagerService.Value.GetUserAvatarFileAsync(documentName, projectId, userId,
                false);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UploadUserAvatarFileAsync(IFormFileCollection files, string account, long projectId)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            await _fileManagerService.Value.UploadUserAvatarFileAsync(files, projectId, userId);

            // TODO: Для чего вообще использовать класс сущности?
            // TODO: С Dapper не нужно все это.
            // TODO: Использовать просто классы DTO для этого, и факторки эти не нужны будут.
            var userAvatarFile = CreateProjectDocumentsFactory.CreateProjectDocuments(files, projectId, null,
                userId);
            
            // Сохраняем файлы проекта.
            await _projectManagmentRepository.CreateProjectTaskDocumentsAsync(userAvatarFile,
                DocumentTypeEnum.ProjectUserAvatar);
            
            // TODO: Тут добавить запись активности пользователя по userId (кто загрузил файл).
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<EpicEntity>> GetEpicsAsync(long projectId)
    {
        try
        {
            var result = await _projectManagmentRepository.GetEpicsAsync(projectId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ProjectManagmentWorkspaceResult> GetBacklogTasksAsync(long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var result = await GetConfigurationWorkSpaceBySelectedTemplateAsync(projectId, account, null,
                ModifyTaskStatuseTypeEnum.Backlog);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<EpicEntity>> GetAvailableEpicsAsync(long projectId)
    {
        try
        {
            var result = await _projectManagmentRepository.GetAvailableEpicsAsync(projectId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task IncludeTaskEpicAsync(long epicId, IEnumerable<string> projectTaskIds, string account,
        string token)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var projectTaskIdToNumbers = projectTaskIds.Select(x => x.GetProjectTaskIdFromPrefixLink());

            await _projectManagmentRepository.IncludeTaskEpicAsync(epicId, projectTaskIdToNumbers);
            
            if (!string.IsNullOrEmpty(token))
            {
                await _projectManagementNotificationService.Value.SendNotifySuccessIncludeEpicTaskAsync(
                    "Все хорошо",
                    "Задачи успешно добавлены в эпик.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);   
            }

            // TODO: Тут добавить запись активности пользователя по userId (кто добавил задачу в эпик).
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);

            if (!string.IsNullOrEmpty(token))
            {
                await _projectManagementNotificationService.Value.SendNotifyErrorIncludeEpicTaskAsync(
                    "Что то пошло не так",
                    "Ошибка при добавлении задач в эпик. Мы уже знаем о проблеме и уже занимаемся ей.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
            }

            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserStoryStatusEntity>> GetUserStoryStatusesAsync()
    {
        var result = await _projectManagmentRepository.GetUserStoryStatusesAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task PlaningSprintAsync(PlaningSprintInput planingSprintInput, string account, string token)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            List<long> projectTaskIds = null;
            if (planingSprintInput.ProjectTaskIds is not null && planingSprintInput.ProjectTaskIds.Any())
            {
                projectTaskIds = GetProjectTaskIdsWithoutPrefix(planingSprintInput.ProjectTaskIds.AsList());
            }
        
            // TODO: Добавить проверку на роль (когда внедрим систему ролей) позволяющую пользователю спланировать спринт.

            // TODO: Реализовать, когда будет реализована логика начала спринта.
            // Автоматически начинаем спринт, если нужно.
            if (planingSprintInput.IsAuthStartSprint)
            {
                // Если не указали даты спринта, но захотели автоматически начать спринт, то это недопустимо.
                if (!planingSprintInput.DateStart.HasValue || !planingSprintInput.DateEnd.HasValue)
                {
                    throw new InvalidOperationException("Не указаны даты спринта. Невозможно начать спринт. " +
                                                        $"ProjectId: {planingSprintInput.ProjectId}. " +
                                                        $"DateStart: {planingSprintInput.DateStart}. " +
                                                        $"DateEnd: {planingSprintInput.DateEnd}.");
                }
            
                // TODO: Добавить проверку, что даты спринта не находятся в прошлом.

                var isQueueSprint = false; // Признак наличия уже активного спринта у проекта.
            
                // TODO: Раскоментить, когда будет реализована логика начала спринта.
                // Проверяем, что уже нет активного спринта. Если он есть уже, то начать автоматически спринт недопустимо.
                // if (expr)
                // {
                //     // Помещаем в очередь бэклога этот спринт.
                //     planingSprintInput.SprintStatus = (int)SprintStatusEnum.Backlog;
                //     
                //     using var transactionScope = _transactionScopeFactory.CreateTransactionScope();
                //     var addedSprintId = await _projectManagmentRepository.PlaningSprintAsync(planingSprintInput);
                //     
                //     // Добавляем задачи в спринт, если их включили в спринт.
                //     if (projectTaskIds is not null && projectTaskIds.Any())
                //     {
                //         await _projectManagmentRepository.IncludeProjectTaskSprintASync(projectTaskIds, addedSprintId);
                //     }
                //
                //     if (isQueueSprint)
                //     {
                //         if (!string.IsNullOrEmpty(token))
                //         {
                //             await _sprintNotificationsService.Value.SendNotifySuccessPlaningSprintAsync("Все хорошо",
                //                 "Спринт успешно спланирован. Проект уже имеет активный спринт." +
                //                 " Спланированный спринт был помещен в очередь бэклога.",
                //                 NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
                //         }
                //     }
                //     
                //     // TODO: Тут добавить запись активности пользователя по userId (кто спланировал спринт).
                //     
                //     transactionScope.Complete();
                // }

                // Автоматическое начало спринта после его планирования.
                // else
                // {
                //     planingSprintInput.SprintStatus = (int)SprintStatusEnum.InWork;
                //
                //     using var transactionScope = _transactionScopeFactory.CreateTransactionScope();
                //     var addedSprintId = await _projectManagmentRepository.PlaningSprintAsync(planingSprintInput);
                //     
                //     // Добавляем задачи в спринт, если их включили в спринт.
                //     if (projectTaskIds is not null && projectTaskIds.Any())
                //     {
                //         await _projectManagmentRepository.IncludeProjectTaskSprintASync(projectTaskIds, addedSprintId);
                //     }
                //
                //     // TODO: Начинать автоматически спринт, только после добавления в спринт задач, если их указали.
                //
                //     if (!string.IsNullOrEmpty(token))
                //     {
                //         await _sprintNotificationsService.Value.SendNotifySuccessPlaningSprintAsync("Все хорошо",
                //             "Спринт успешно спланирован. Спринт автоматически был начат." +
                //             " Его можно увидеть в активных спринтах.",
                //             NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
                //     }
                //
                //     // TODO: Тут добавить запись активности пользователя по userId (кто спланировал спринт).
                //
                //     transactionScope.Complete();
                // }
            }

            // Обычное создание спринта (без его автоматического начала).
            else
            {
                planingSprintInput.SprintStatus = (int)SprintStatusEnum.Backlog;
                
                using var transactionScope = _transactionScopeFactory.CreateTransactionScope();
                var addedSprintId = await _projectManagmentRepository.PlaningSprintAsync(planingSprintInput);
                
                // Добавляем задачи в спринт, если их включили в спринт.
                if (projectTaskIds is not null && projectTaskIds.Any())
                {
                    await _projectManagmentRepository.IncludeProjectTaskSprintAsync(projectTaskIds, addedSprintId);
                }

                if (planingSprintInput.DateStart.HasValue && planingSprintInput.DateEnd.HasValue)
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        await _projectManagementNotificationService.Value.SendNotifySuccessPlaningSprintAsync(
                            "Все хорошо",
                            "Спринт успешно спланирован. Теперь его можно начать.",
                            NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);   
                    }
                }
                
                // TODO: Тут добавить запись активности пользователя по userId (кто спланировал спринт).
                
                transactionScope.Complete();
            }
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<TaskSprintOutput> GetSprintTaskAsync(long projectId, string projectTaskId)
    {
        try
        {
            var result = await _projectManagmentRepository.GetSprintTaskAsync(projectId,
                projectTaskId.GetProjectTaskIdFromPrefixLink());

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TaskSprintOutput>> GetAvailableProjectSprintsAsync(long projectId,
        string projectTaskId)
    {
        try
        {
            var result = await _projectManagmentRepository.GetAvailableProjectSprintsAsync(projectId,
                projectTaskId.GetProjectTaskIdFromPrefixLink());

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<EpicTaskResult> GetEpicTasksAsync(long projectId, long epicId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            // TODO: Этот код дублируется в этом сервисе. Вынести в приватный метод и кортежем вернуть нужные данные.
            // Получаем настройки проекта.
            var projectSettings = await _projectSettingsConfigRepository.GetProjectSpaceSettingsByProjectIdAsync(
                projectId, userId);
            var projectSettingsItems = projectSettings?.AsList();

            if (projectSettingsItems is null
                || !projectSettingsItems.Any()
                || projectSettingsItems.Any(x => x is null))
            {
                throw new InvalidOperationException("Ошибка получения настроек проекта. " +
                                                    $"ProjectId: {projectId}. " +
                                                    $"UserId: {userId}");
            }

            var template = projectSettingsItems.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_TEMPLATE_ID));
            var templateId = Convert.ToInt32(template!.ParamValue);

            var result = await _projectManagmentRepository.GetEpicTasksAsync(projectId, epicId, templateId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task InsertOrUpdateTaskSprintAsync(long sprintId, string projectTaskId)
    {
        await _projectManagmentRepository.InsertOrUpdateTaskSprintAsync(sprintId,
            projectTaskId.GetProjectTaskIdFromPrefixLink());
            
        // TODO: Тут добавить запись активности пользователя по userId (кто обновил спринт задачи).
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод модифицирует выходной результат.
    /// </summary>
    /// <param name="projectManagmentTaskStatusTemplates">Список статусов, каждый статус может включать
    /// или не включать в себя задачи.</param>
    /// <param name="tasks">Список задач рабочего пространства проекта.</param>
    /// <param name="projectId">Id проекта</param>
    /// <param name="strategy">Выбранная пользователем стратегия представления.</param>
    /// <param name="paginatorStatusId">Id статуса, для которого нужно применить пагинатор.</param>
    /// <param name="page">Номер страницы.</param>
    /// <exception cref="InvalidOperationException">Может бахнуть, если какое-то условие не прошли.</exception>
    private async Task ModifyProjectManagmentTaskStatusesResultAsync(
        IEnumerable<ProjectManagmentTaskStatusTemplateOutput> projectManagmentTaskStatusTemplates,
        List<ProjectTaskExtendedEntity> tasks, long projectId, string strategy, int? paginatorStatusId = null,
        int page = 1)
    {
        // Получаем имена авторов задач.
        var authorIds = tasks.Select(x => x.AuthorId);
        var authors = await _userRepository.GetAuthorNamesByAuthorIdsAsync(authorIds);

        if (authors.Count == 0)
        {
            throw new InvalidOperationException("Не удалось получить авторов задач.");
        }

        var notValidExecutors = tasks.Where(x => x.ExecutorId <= 0);

        // Обязательно логируем такое если обнаружили, но не стопаем выполнение логики.
        if (notValidExecutors.Any())
        {
            var ex = new InvalidOperationException(
                "Найдены невалидные исполнители задач." +
                $" ExecutorIds: {JsonConvert.SerializeObject(notValidExecutors)}." +
                $" ProjectId: {projectId}");

            _logger.LogError(ex, ex.Message);

            // Отправляем ивент в пачку.
            await _pachcaService.SendNotificationErrorAsync(ex);
        }

        // Получаем имена исполнителей задач.
        var executorIds = tasks.Where(x => x.ExecutorId > 0).Select(x => x.ExecutorId);
        var executors = await _userRepository.GetExecutorNamesByExecutorIdsAsync(executorIds);

        if (executors.Count == 0)
        {
            throw new InvalidOperationException("Не удалось получить исполнителей задач.");
        }

        IDictionary<int, ProjectTagOutput> tags = null;

        // Если есть теги, то пойдем получать.
        if (tasks.Any(x => x.TagIds is not null))
        {
            var tagIds = tasks.Where(x => x.TagIds is not null).SelectMany(x => x.TagIds).Distinct();
            tags = await _projectManagmentRepository.GetTagNamesByTagIdsAsync(tagIds);
        }

        var typeIds = tasks.Select(x => x.TaskTypeId);
        var types = await _projectManagmentRepository.GetTypeNamesByTypeIdsAsync(typeIds);

        var statusIds = tasks.Select(x => x.TaskStatusId);
        var statuseNames = (await _projectManagmentTemplateRepository.GetTaskTemplateStatusesAsync(statusIds))
            .ToList();

        var resolutionIds = tasks
            .Where(x => x.ResolutionId is not null)
            .Select(x => (int)x.ResolutionId)
            .ToList();

        IDictionary<int, TaskResolutionOutput> resolutions = null;

        // Если есть резолюции задач, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у задач.
        if (resolutionIds.Any())
        {
            resolutions = await _projectManagmentRepository.GetResolutionNamesByResolutionIdsAsync(
                resolutionIds);
        }
        
        var priorityIds = tasks
            .Where(x => x.PriorityId is not null)
            .Select(x => (int)x.PriorityId)
            .ToList();
        
        IDictionary<int, TaskPriorityOutput> priorities = null;
        
        // Если есть приоритеты задач, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у задач.
        if (priorityIds.Any())
        {
            priorities = await _projectManagmentRepository.GetPriorityNamesByPriorityIdsAsync(
                priorityIds);
        }
        
        var watcherIds = tasks
            .Where(x => x.WatcherIds is not null)
            .SelectMany(x => x.WatcherIds)
            .ToList();

        IDictionary<long, UserInfoOutput> watchers = null;

        // Если есть наблюдатели, пойдем получать их.
        // Если каких то нет, не страшно, значит они не заполнены у задач.
        if (watcherIds.Any())
        {
            watchers = await _userRepository.GetWatcherNamesByWatcherIdsAsync(watcherIds);
        }

        // Распределяем задачи по статусам.
        foreach (var ps in projectManagmentTaskStatusTemplates)
        {
            var tasksByStatus = tasks
                .Where(s => s.TaskStatusId == ps.TaskStatusId)
                .OrderByDescending(o => o.Created); // Новые задачи статуса будут выше.

            // Для этого статуса нет задач, пропускаем.
            if (!tasksByStatus.Any())
            {
                continue;
            }

            var mapTasks = _mapper.Map<List<ProjectManagmentTaskOutput>>(tasksByStatus);

            // Добавляем задачи статуса, если есть что добавлять.
            if (mapTasks.Any())
            {
                var userEmails = await _userRepository.GetUserEmailByUserIdsAsync(mapTasks.Select(x => x.ExecutorId));
                var avatarFiles = await _userService.GetUserAvatarFilesAsync(projectId, userEmails);
                
                // Записываем названия исходя от Id полей.
                foreach (var ts in mapTasks)
                {
                    // ФИО автора задачи.
                    ts.AuthorName = authors.TryGet(ts.AuthorId)?.FullName;

                    var executorId = ts.ExecutorId;

                    if (executorId <= 0)
                    {
                        throw new InvalidOperationException("Невалидный исполнитель задачи." +
                                                            $" ExecutorId: {executorId}");
                    }

                    // Заполняем данные исполнителя задачи (если назначен).
                    var executorName = executors.TryGet(ts.ExecutorId)?.FullName;
                    ts.Executor = new Executor
                    {
                        ExecutorName = executorName
                    };

                    if (!avatarFiles.ContainsKey(ts.ExecutorId))
                    {
                        // Получаем дефолтное изображение.
                        ts.Executor.Avatar = avatarFiles.TryGet(0);
                    }

                    else
                    {
                        ts.Executor.Avatar = avatarFiles.TryGet(ts.ExecutorId);
                    }

                    var taskStatusName = statuseNames.Find(x => x.StatusId == ts.TaskStatusId)?.StatusName;

                    if (string.IsNullOrEmpty(taskStatusName))
                    {
                        throw new InvalidOperationException($"Не удалось получить TaskStatusName: {ts.TaskStatusId}.");
                    }

                    // Название статуса.
                    ts.TaskStatusName = taskStatusName;

                    // Название типа задачи.
                    ts.TaskTypeName = types.TryGet(ts.TaskTypeId)?.TypeName;

                    // Название тегов (меток) задачи.
                    if (tags is not null && tags.Count > 0)
                    {
                        foreach (var tg in ts.TagIds)
                        {
                            ts.TagNames = new List<string> { tags.TryGet(tg)?.TagName };
                        }
                    }

                    // Название резолюции задачи.
                    if (resolutions is not null && resolutions.Count > 0)
                    {
                        var resolutionName = resolutions.TryGet(ts.ResolutionId);

                        // Если резолюции нет, не страшно. Значит не указана у задачи.
                        if (resolutionName is not null)
                        {
                            ts.ResolutionName = resolutions.TryGet(ts.ResolutionId)?.ResolutionName;
                        }
                    }

                    // Названия наблюдателей задачи.
                    if (watchers is not null && watchers.Count > 0)
                    {
                        foreach (var w in ts.WatcherIds)
                        {
                            ts.WatcherNames = new List<string> { watchers.TryGet(w)?.FullName };
                        }
                    }
                    
                    // Название приорита задачи.
                    if (priorities is not null && priorities.Count > 0)
                    {
                        var priorityName = priorities.TryGet(ts.PriorityId);

                        // Если приоритета нет, не страшно. Значит не указана у задачи.
                        if (priorityName is not null)
                        {
                            ts.PriorityName = priorities.TryGet(ts.PriorityId)?.PriorityName;
                        }
                    }
                }
                
                var statusTasksCount = mapTasks.Count; // Всего задач у статуса.
                var isAppended = false;

                // Действия с задачами, если стратегия представления Scrum.
                if (strategy.Equals("sm"))
                {
                    if (mapTasks.Count > 10)
                    {
                        // При нажатии на кнопку "Показать больше", докидываем в список задач определенного
                        // статуса следующие 10.
                        if (paginatorStatusId.HasValue && paginatorStatusId == ps.TaskStatusId)
                        {
                            ps.ProjectManagmentTasks = new List<ProjectManagmentTaskOutput>(statusTasksCount);
                            
                            // TODO: Надо переделать на IQueryable, чтобы не работать со всем массивом данных.
                            // Разбиваем пагинатором следующие 10 задач, которые будем докидывать в список.
                            var appendedTasks = mapTasks
                                .Skip((page - 1) * _scrumPageSize)
                                .Take(_scrumPageSize)
                                .AsList();
                            var appendedTaskIds = appendedTasks.Select(x => x.TaskId).AsList();

                            foreach (var t in mapTasks)
                            {
                                // Наполняем уже существующий список определенного статуса.
                                if (!appendedTaskIds.Contains(t.TaskId))
                                {
                                    ps.ProjectManagmentTasks.Add(t);
                                }
                            }

                            // Добавляем новые 10 задач к тем, что уже выведены на фронте.
                            ps.ProjectManagmentTasks.AddRange(appendedTasks);
                            isAppended = true;
                        }

                        // Иначе применить пагинатор для всех статусов шаблона.
                        else if (!paginatorStatusId.HasValue)
                        {
                            // TODO: Надо переделать на IQueryable, чтобы не работать со всем массивом данных.
                            mapTasks = mapTasks
                                .Skip((page - 1) * _scrumPageSize)
                                .Take(_scrumPageSize)
                                .AsList();
                        }
                    }
                }
                
                // Заполняем модель пагинатора для фронта, чтобы он кидал последующие запросы с параметрами страниц.
                ps.Paginator = new Paginator(statusTasksCount, page, _scrumPageSize);

                // Если задачи уже докидывались, не добавляем снова.
                if (!isAppended)
                {
                    // Добавляем задачи статуса.
                    ps.ProjectManagmentTasks = new List<ProjectManagmentTaskOutput>(statusTasksCount);
                    ps.ProjectManagmentTasks.AddRange(mapTasks);
                }
                
                // Сортируем задачи статуса по убываниию даты создания. Новые задачи будут выше.
                ps.ProjectManagmentTasks = ps.ProjectManagmentTasks.OrderByDescending(o => o.Created).AsList();

                // Кол-во задач в статусе.
                ps.Total = statusTasksCount;
            }
        }
    }

    /// <summary>
    /// Метод модифицирует результаты связей задачи.
    /// </summary>
    /// <param name="links">Связи задач.</param>
    /// <returns>Модифицированные данные связей.</returns>
    private async Task<IEnumerable<GetTaskLinkOutput>> ModifyTaskLinkResultAsync(List<ProjectTaskExtendedEntity> tasks)
    {
        var result = new List<GetTaskLinkOutput>();
        
        // Получаем имена исполнителей задач.
        var executorIds = tasks.Where(x => x.ExecutorId > 0).Select(x => x.ExecutorId);
        var executors = await _userRepository.GetExecutorNamesByExecutorIdsAsync(executorIds);

        if (executors.Count == 0)
        {
            throw new InvalidOperationException("Не удалось получить исполнителей задач.");
        }
        
        var statusIds = tasks.Select(x => x.TaskStatusId);
        var statuseNames = (await _projectManagmentTemplateRepository.GetTaskTemplateStatusesAsync(statusIds))
            .ToList();

        // Наполняем выходные данные задачи.
        foreach (var t in tasks)
        {
            var link = new GetTaskLinkOutput
            {
                ExecutorName = executors.TryGet(t.ExecutorId).FullName,
                TaskName = t.Name,
                TaskStatusName = statuseNames.Find(x => x.StatusId == t.TaskStatusId)?.StatusName,
                LastUpdated = t.Updated?.ToString("f"), // Например, 17 июля 2015 г. 17:04
                ProjectTaskId = t.ProjectTaskId,
                PriorityId = t.PriorityId!.Value,
                TaskId = t.TaskId,
                TaskIdPrefix = t.TaskIdPrefix
            };

            result.Add(link);
        }

        return result;
    }

    /// <summary>
    /// Метод получает из префиксных Id задач в рамках проекта только число,
    /// чтобы в итоге работать именно с числовыми Id задач в рамках проекта. 
    /// </summary>
    /// <param name="projectTaskIds">Массив префиксных Id задач в рамках проекта. </param>
    /// <returns>Id задач в рамках проекта только число.</returns>
    private List<long> GetProjectTaskIdsWithoutPrefix(List<string> projectTaskIds)
    {
        var result = new List<long>(projectTaskIds.Count);
        result.AddRange(projectTaskIds.Select(pti => pti.GetProjectTaskIdFromPrefixLink()));

        return result;
    }

    #endregion
}