using System.Runtime.CompilerServices;
using AutoMapper;
using Dapper;
using FluentValidation.Results;
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
using LeokaEstetica.Platform.Database.MongoDb.Abstractions;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Integrations.Abstractions.Reverso;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
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
using SearchAgileObjectTypeEnum = LeokaEstetica.Platform.Models.Enums.SearchAgileObjectTypeEnum;

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
    private readonly IDiscordService _discordService;
    private readonly IProjectManagmentTemplateRepository _projectManagmentTemplateRepository;
    private readonly ITransactionScopeFactory _transactionScopeFactory;
    private readonly IProjectSettingsConfigRepository _projectSettingsConfigRepository;
    private readonly Lazy<IReversoService> _reversoService;
    private readonly Lazy<IFileManagerService> _fileManagerService;
    private readonly IUserService _userService;
    private readonly Lazy<IDistributionStatusTaskService> _distributionStatusTaskService;
    private readonly IProjectManagementTemplateService _projectManagementTemplateService;
    private readonly Lazy<IProjectManagmentRoleRepository> _projectManagmentRoleRepository;
    private readonly IMongoDbRepository _mongoDbRepository;

    /// <summary>
    /// Статусы задач, которые являются самыми базовыми и никогда не меняются независимо от шаблона проекта.
    /// Новые статусы обязательно должны ассоциироваться с одним из перечисленных системных названий.
    /// </summary>
    private readonly List<string> _associationStatusSysNames = new()
    {
        "New", "InWork", "InDevelopment", "Completed"
    };
    
    private readonly Lazy<IHubNotificationService> _hubNotificationService;

    /// <summary>
    /// Конструктор.
    /// <param name="logger">Логгер.</param>
    /// <param name="projectManagmentRepository">Репозиторий управления проектами.</param>
    /// <param name="mapper">Маппер.</param>
    /// <param name="mapper">Репозиторий пользователей.</param>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="projectManagmentTemplateRepository">Репозиторий шаблонов проектов.</param>
    /// <param name="transactionScopeFactory">Факторка транзакций.</param>
    /// <param name="projectSettingsConfigRepository">Репозиторий настроек проектов.</param>
    /// <param name="projectSettingsConfigRepository">Сервис транслитера.</param>
    /// <param name="fileManagerService">Сервис менеджера файлов.</param>
    /// <param name="userService">Сервис пользователей.</param>
    /// <param name="distributionStatusTaskService">Сервис распределение задач по статусам.</param>
    /// <param name="projectManagementTemplateService">Сервис шаблонов проекта.</param>
    /// <param name="projectManagmentRoleRepository">Репозиторий ролей проекта.</param>
    /// <param name="mongoDbRepository">Репозиторий MongoDB.</param>
    /// <param name="hubNotificationService">Сервис уведомлений хабов.</param>
    /// </summary>
    public ProjectManagmentService(ILogger<ProjectManagmentService> logger,
        IProjectManagmentRepository projectManagmentRepository,
        IMapper mapper,
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        IDiscordService discordService,
        IProjectManagmentTemplateRepository projectManagmentTemplateRepository,
        ITransactionScopeFactory transactionScopeFactory,
        IProjectSettingsConfigRepository projectSettingsConfigRepository,
        Lazy<IReversoService> reversoService,
        Lazy<IFileManagerService> fileManagerService,
        IUserService userService,
        Lazy<IDistributionStatusTaskService> distributionStatusTaskService,
        IProjectManagementTemplateService projectManagementTemplateService,
        Lazy<IProjectManagmentRoleRepository> projectManagmentRoleRepository,
        IMongoDbRepository mongoDbRepository,
         Lazy<IHubNotificationService> hubNotificationService)
    {
        _logger = logger;
        _projectManagmentRepository = projectManagmentRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _discordService = discordService;
        _projectManagmentTemplateRepository = projectManagmentTemplateRepository;
        _transactionScopeFactory = transactionScopeFactory;
        _projectSettingsConfigRepository = projectSettingsConfigRepository;
        _reversoService = reversoService;
        _fileManagerService = fileManagerService;
        _userService = userService;
        _distributionStatusTaskService = distributionStatusTaskService;
        _projectManagementTemplateService = projectManagementTemplateService;
        _projectManagmentRoleRepository = projectManagmentRoleRepository;
        _mongoDbRepository = mongoDbRepository;
        _hubNotificationService = hubNotificationService;
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
                            IsFooterItem = x.IsFooterItem,
                            Visible = x.Visible
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
                            IsFooterItem = x.IsFooterItem,
                            Visible = x.Visible
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
                        var filters = mapItems.Items.Where(x => x.Visible).OrderBy(o => o.Position);

                        var selectedFilters = filters.Select(x => new Panel
                        {
                            Label = x.ItemName,
                            Id = x.Id,
                            Disabled = x.Disabled,
                            IsFooterItem = x.IsFooterItem,
                            Visible = x.Visible
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
                            IsFooterItem = x.IsFooterItem,
                            Visible = x.Visible
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
                            IsFooterItem = x.IsFooterItem,
                            Visible = x.Visible
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
                            IsFooterItem = x.IsFooterItem,
                            Visible = x.Visible
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
            
            var ifProjectMember = await _projectRepository.CheckExistsProjectTeamMemberAsync(projectId, userId);

            if (!ifProjectMember)
            {
                var ex = new InvalidOperationException(
                    "Была попытка просмотра раб.пространства проекта без наличия доступа. " +
                    "Сработала система запрета доступа. " +
                    $"UserId: {userId} не имеет доступа. " +
                    $"ProjectId: {projectId}");
                
                await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                
                return new ProjectManagmentWorkspaceResult { IsAccess = false };
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
            // Получаем список шаблонов задач, которые пользователь может выбрать перед переходом в рабочее пространство.
            var items = await _projectManagementTemplateService.GetProjectManagmentTemplatesAsync(templateId);
            var templateStatusesItems = _mapper.Map<IEnumerable<ProjectManagmentTaskTemplateResult>>(items);
            var statuses = templateStatusesItems?.AsList();

            if (statuses is null || !statuses.Any())
            {
                throw new InvalidOperationException("Не удалось получить набор статусов шаблона." +
                                                    $" TemplateId: {templateId}." +
                                                    $"ProjectId: {projectId}.");
            }

            // Проставляем Id шаблона статусам.
            await _projectManagementTemplateService.SetProjectManagmentTemplateIdsAsync(statuses);

            // Получаем выбранную пользователем стратегию представления.
            var strategy = await _projectManagmentRepository.GetProjectUserStrategyAsync(projectId, userId);

            _logger?.LogInformation(
                $"Начали получение списка задач для рабочего пространства для проекта {projectId}");

            // Получаем задачи пользователя, которые принадлежат проекту в рабочем пространстве.
            var projectTasks = await _projectManagmentRepository.GetProjectTasksAsync(projectId, strategy!);

            _logger?.LogInformation(
                $"Закончили получение списка задач для рабочего пространства для проекта {projectId}.");
            
            var tasks = projectTasks?.AsList();
            
            var result = new ProjectManagmentWorkspaceResult
            {
                ProjectManagmentTaskStatuses = statuses.First().ProjectManagmentTaskStatusTemplates
                    .Where(x => x.TemplateId == templateId),
                Strategy = strategy!
            };

            // Если задачи есть, то модифицируем выходные данные.
            if (tasks is not null && tasks.Any())
            {
                // Распределяем задачи по статусам и модифицируем выходные результаты.
                await _distributionStatusTaskService.Value.DistributionStatusTaskAsync(
                    result.ProjectManagmentTaskStatuses, tasks, modifyTaskStatuseType, projectId, paginatorStatusId,
                    result.Strategy, page);
            }

            result.IsAccess = true;

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
            // Если переданный тип неизвестен ,например, если вставили просто ссылку в url - то найдем в БД тип задачи.
            if (taskDetailType == TaskDetailTypeEnum.Undefined)
            {
                var taskType = await _projectManagmentRepository.GetTaskTypeByProjectIdProjectTaskIdAsync(projectId,
                        projectTaskId.GetProjectTaskIdFromPrefixLink());
                
                // Если все же не удалось определить тип задачи.
                if (taskType == TaskDetailTypeEnum.Undefined)
                {
                    throw new InvalidOperationException("Неизвестный тип детализации. " +
                                                        " Заполнение Agile-объекта не будет происходить. " +
                                                        $"TaskType: {taskType}.");
                }

                taskDetailType = taskType;
            }
            
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var ifProjectMember = await _projectRepository.CheckExistsProjectTeamMemberAsync(projectId, userId);

            if (!ifProjectMember)
            {
                var ex = new InvalidOperationException("Была попытка просмотра задачи без наличия доступа. " +
                                                       "Сработала система запрета доступа. " +
                                                       $"UserId: {userId} не имеет доступа. " +
                                                       $"ProjectId: {projectId}");
                
                await _discordService.SendNotificationErrorAsync(ex).ConfigureAwait(false);
                
                return new ProjectManagmentTaskOutput { IsAccess = false };
            }
            
            var builderData = new AgileObjectBuilderData(_projectManagmentRepository, _userRepository,
                _discordService, _userService, _projectManagmentTemplateRepository, _mapper,
                projectTaskId.GetProjectTaskIdFromPrefixLink(), projectId, _projectSettingsConfigRepository);
                
            AgileObjectBuilder? builder = null;
 
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
            if (taskDetailType is TaskDetailTypeEnum.Story)
            {
                // Настраиваем билдер для построения истории.
                builder = new UserStoryBuilder { BuilderData = builderData };
            }

            if (builder is null)
            {
                throw new InvalidOperationException("Тип билдера не определен. Построения не будет происходить. " +
                                                    $"TaskDetailTypeEnum: {taskDetailType}");
            }
            
            var agileObject = new AgileObject();

            // Запускаем построение нужного Agile-объекта.
            await agileObject.BuildAsync(builder, taskDetailType);
            
            builder.ProjectManagmentTask.IsAccess = true;
                
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

            // Проверяем, есть ли уже такая задача у такого проекта.
            // Проверка на дубли идет по совпадению типа задачи и названия задачи.
            var ifExists = await _projectManagmentRepository.IfExistsProjectTaskAsync(projectManagementTaskInput.Name,
                projectManagementTaskInput.TaskTypeId, projectId);
                
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            if (ifExists)
            {
                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    $"Такая задача уже заведена в проекте {projectId}.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotifyWarningDublicateProjectTask",
                    userCode, UserConnectionModuleEnum.ProjectManagement);

                // TODO: Переделать на уведомление через хаб.
                return new CreateProjectManagementTaskOutput
                {
                    Errors = new List<ValidationFailure>
                    {
                        new("name", "Дубликат задачи в системе.")
                    }
                };
            }
            
            // TODO: Этот код дублируется в этом сервисе. Вынести в приватный метод и кортежем вернуть нужные данные.
            // Получаем настройки проекта.
            var projectSettings = await _projectSettingsConfigRepository.GetProjectSpaceSettingsByProjectIdAsync(
                projectId, userId);
            var projectSettingsItems = projectSettings.AsList();

            if (projectSettingsItems is null || projectSettingsItems.Count == 0)
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
            
            var parseTaskType = Enum.GetName((SearchAgileObjectTypeEnum)projectManagementTaskInput.TaskTypeId);
            var taskType = Enum.Parse<SearchAgileObjectTypeEnum>(parseTaskType!);

            // Если идет создание задачи или ошибки.
            if (new[] { SearchAgileObjectTypeEnum.Task, SearchAgileObjectTypeEnum.Error }.Contains(taskType))
            {
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
            if (taskType == SearchAgileObjectTypeEnum.Epic)
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
            if (taskType == SearchAgileObjectTypeEnum.Story)
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

            var maxUserTagPosition = await _projectManagmentRepository.GetLastPositionProjectTagAsync(projectId);
            
            // TODO: Для чего вообще использовать класс сущности?
            // TODO: С Dapper не нужно все это.
            // TODO: Использовать просто классы DTO для этого, и факторки эти не нужны будут.
            var projectTag = CreateUserTaskTagFactory.CreateProjectTag(tagName, tagDescription, tagSysName,
                    ++maxUserTagPosition, projectId);
            await _projectManagmentRepository.CreateProjectTaskTagAsync(projectTag);
            
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Метка добавлена в проект.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotifySuccessCreateProjectTag", userCode,
                UserConnectionModuleEnum.ProjectManagement);
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
                result = result.DistinctBy(d => d.StatusSysName).AsList();
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
            TaskDetailTypeEnum taskType;
            
            // Если переданный тип неизвестен ,например, если вставили просто ссылку в url - то найдем в БД тип задачи.
            if (Enum.Parse<TaskDetailTypeEnum>(taskDetailType.ToPascalCase()) == TaskDetailTypeEnum.Undefined
                || Enum.Parse<TaskDetailTypeEnum>(taskDetailType) == TaskDetailTypeEnum.Undefined)
            {
                var findTaskType = await _projectManagmentRepository.GetTaskTypeByProjectIdProjectTaskIdAsync(
                    projectId, projectTaskId.GetProjectTaskIdFromPrefixLink());
                
                // Если все же не удалось определить тип задачи.
                if (findTaskType == TaskDetailTypeEnum.Undefined)
                {
                    throw new InvalidOperationException("Неизвестный тип детализации. " +
                                                        " Заполнение Agile-объекта не будет происходить. " +
                                                        $"TaskType: {findTaskType}.");
                }

                taskType = findTaskType;
            }

            // Иначе просто парсим.
            else
            {
                taskType = Enum.Parse<TaskDetailTypeEnum>(taskDetailType);
            }
            
            var onlyProjectTaskId = projectTaskId.GetProjectTaskIdFromPrefixLink();
            bool ifProjectHavingTask;
            long currentTaskStatusId = 0;
            var transitionType = TransitionTypeEnum.None;

            if (taskType is TaskDetailTypeEnum.Task or TaskDetailTypeEnum.Error)
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

            if (taskType == TaskDetailTypeEnum.Epic)
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
            
            if (taskType == TaskDetailTypeEnum.Story)
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

            // Получаем все переходы из промежуточной таблицы отталкиваясь от текущего статуса задачи (конкретного типа).
            var statusIds = (await _projectManagmentRepository
                    .GetProjectManagementTransitionIntermediateTemplatesAsync(currentTaskStatusId, transitionType,
                        templateId))
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
                        TaskStatusId = ts.TaskStatusId,
                        AvailableStatusSysName = userStatus.StatusSysName
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
                    TaskStatusId = ts.TaskStatusId,
                    AvailableStatusSysName = commonStatuse.StatusSysName
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
                    TaskStatusId = currentTaskStatus.TaskStatusId,
                    AvailableStatusSysName = currentTaskStatus.StatusSysName
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
                    TaskStatusId = x.StatusId,
                    AvailableStatusSysName = currentTaskStatus.StatusSysName
                }));
                
                result = await RemoveTransitionStatusesAsync(result, transitionType);
            }
            
            // Дополняем статусами, в зависимости от типа задачи.
            // Если нужно получить доступные статусы (переходы) для истории.
            if (transitionType == TransitionTypeEnum.History)
            {
                // TODO: Если в будущем будет функционал для создания кастомных статусов истории пользователем,
                // TODO: то придется заводить поле TaskStatusId в таблице статусов историй и тогда его тут получать уже.
                // Сейчас StatusId и TaskStatusId у историй одинаковые будут, так как нет отдельного поля под TaskStatusId у них,
                // потому что создание кастомных статусов для историй пока не предполагается в системе.
                var storyStatuses = await _projectManagmentRepository.GetUserStoryStatusesAsync();
                result.AddRange(storyStatuses.Select(x => new AvailableTaskStatusTransitionOutput
                {
                    StatusName = x.StatusName,
                    StatusId = x.StatusId,
                    TaskStatusId = x.StatusId,
                    AvailableStatusSysName = currentTaskStatus.StatusSysName
                }));

                result = await RemoveTransitionStatusesAsync(result, transitionType);
            }

            return result.OrderBy(x => x.StatusId);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task ChangeTaskStatusAsync(long projectId, string changeStatusId, string taskId,
        string taskDetailType, string account)
    {
        try
        {
            var detailType = Enum.Parse<TaskDetailTypeEnum>(taskDetailType);
            var onlyTaskId = taskId.GetProjectTaskIdFromPrefixLink();
            
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
        
            switch (detailType)
            {
                case TaskDetailTypeEnum.Task or TaskDetailTypeEnum.Error:
                    await _projectManagmentRepository.ChangeTaskStatusAsync(projectId,
                        changeStatusId.GetProjectTaskIdFromPrefixLink(), onlyTaskId);
                    break;

                case TaskDetailTypeEnum.Epic:
                    // Проверяем, допустимо ли менять на такой статус.
                    var ifExistsEpicStatus = await _projectManagmentRepository.IfEpicAvailableStatusAsync(
                            changeStatusId.GetProjectTaskIdFromPrefixLink());

                    // Недопустимо, стопаем выполнение логики и уведомляем фронт.
                    if (!ifExistsEpicStatus)
                    {
                        await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                            "Нельзя перевести эпик в указанный статус.",
                            NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotifyWarningChangeEpicStatus",
                            userCode, UserConnectionModuleEnum.ProjectManagement);

                        break;
                    }
                    
                    await _projectManagmentRepository.ChangeEpicStatusAsync(projectId,
                        changeStatusId.GetProjectTaskIdFromPrefixLink(), onlyTaskId);
                    break;
            
                case TaskDetailTypeEnum.Story:
                    // Проверяем, допустимо ли менять на такой статус.
                    var ifExistsStoryStatus = await _projectManagmentRepository.IfStoryAvailableStatusAsync(
                        changeStatusId.GetProjectTaskIdFromPrefixLink());

                    // Недопустимо, стопаем выполнение логики и уведомляем фронт.
                    if (!ifExistsStoryStatus)
                    {
                        await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                            "Нельзя перевести историю в указанный статус.",
                            NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotifyWarningChangeStoryStatus",
                            userCode, UserConnectionModuleEnum.ProjectManagement);

                        break;
                    }
                    
                    await _projectManagmentRepository.ChangeStoryStatusAsync(projectId,
                        changeStatusId.GetProjectTaskIdFromPrefixLink(), onlyTaskId);
                    break;
                
                case TaskDetailTypeEnum.Sprint:
                    await _projectManagmentRepository.ChangeSprintStatusAsync(projectId,
                        changeStatusId.GetProjectTaskIdFromPrefixLink(), onlyTaskId);
                    break;
            }
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
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
            // TaskFromLink - Id задачи в рамках проекта (без префикса).
            await _projectManagmentRepository.CreateTaskLinkAsync(currentTask.TaskId,
                taskLinkInput.TaskToLink.GetProjectTaskIdFromPrefixLink(), taskLinkInput.LinkType, projectId,
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
            
            // TODO: Юзать как Lazy, когда зарегаем в автофаке.
            var documentIds = await _mongoDbRepository.UploadFilesAsync(files, projectId,
                taskId.GetProjectTaskIdFromPrefixLink());

            // TODO: Для чего вообще использовать класс сущности?
            // TODO: С Dapper не нужно все это.
            // TODO: Использовать просто классы DTO для этого, и факторки эти не нужны будут.
            var projectTaskFiles = CreateProjectDocumentsFactory.CreateProjectDocuments(files, projectId,
                taskId.GetProjectTaskIdFromPrefixLink(), userId);
            
            // Сохраняем файлы задачи проекта.
            await _projectManagmentRepository.CreateProjectTaskDocumentsAsync(projectTaskFiles,
                DocumentTypeEnum.ProjectTask, documentIds.AsList());

            // TODO: Тут добавить запись активности пользователя по userId (писать кто добавил файл).
        }
        
        catch (Exception ex)
        {
            await _discordService.SendNotificationErrorAsync(ex);
            
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
            
            // TODO: Юзать как Lazy, когда зарегаем в автофаке.
            // Скачиваем документ из MongoDB.
            var result = await _mongoDbRepository.DownloadFileAsync(documentName, projectId, task.ProjectTaskId);

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
            // Если переданный тип неизвестен ,например, если вставили просто ссылку в url - то найдем в БД тип задачи.
            if (taskDetailType == TaskDetailTypeEnum.Undefined)
            {
                var taskType = await _projectManagmentRepository.GetTaskTypeByProjectIdProjectTaskIdAsync(projectId,
                    projectTaskId.GetProjectTaskIdFromPrefixLink());
                
                // Если все же не удалось определить тип задачи.
                if (taskType == TaskDetailTypeEnum.Undefined)
                {
                    throw new InvalidOperationException("Неизвестный тип детализации. " +
                                                        " Заполнение Agile-объекта не будет происходить. " +
                                                        $"TaskType: {taskType}.");
                }

                taskDetailType = taskType;
            }
            
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

                taskId = task.ProjectTaskId;
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
                
                taskId = task.ProjectEpicId;
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
    public async Task RemoveTaskFileAsync(string? mongoDocumentId)
    {
        try
        {
            await _projectManagmentRepository.RemoveDocumentAsync(mongoDocumentId);
            
            // TODO: Юзать как Lazy, когда зарегаем в автофаке.
            // Удаляем файл в БД.
            await _mongoDbRepository.RemoveFileAsync(mongoDocumentId);
            
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

            ProjectStrategyEnum shortSysName;

            switch (strategySysName)
            {
                case "Kanban":
                    shortSysName = ProjectStrategyEnum.Kn;
                    break;

                case "Scrum":
                    shortSysName = ProjectStrategyEnum.Sm;
                    break;

                default:
                    throw new InvalidOperationException($"Неизвестный тип стратегии: {strategySysName}.");
            }

            // Фиксируем выбранную пользователем стратегию представления в БД.
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
            
            var documentId = await _mongoDbRepository.UploadUserAvatarFileAsync(files);

            // TODO: Для чего вообще использовать класс сущности?
            // TODO: С Dapper не нужно все это.
            // TODO: Использовать просто классы DTO для этого, и факторки эти не нужны будут.
            var userAvatarFile = CreateProjectDocumentsFactory.CreateProjectDocuments(files, projectId, null,
                userId);
            
            // Сохраняем файлы проекта.
            await _projectManagmentRepository.CreateProjectTaskDocumentsAsync(userAvatarFile,
                DocumentTypeEnum.ProjectUserAvatar, new List<string?> { documentId });
            
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
    public async Task IncludeTaskEpicAsync(long projectEpicId, IEnumerable<string> projectTaskIds, string account,
        long projectId)
    {
        var userId = await _userRepository.GetUserByEmailAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }
        
        var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
        
        try
        {
            var projectTaskIdToNumbers = projectTaskIds.Select(x => x.GetProjectTaskIdFromPrefixLink());

            var epicId = await _projectManagmentRepository.GetEpicIdByProjectEpicIdAsync(projectId, projectEpicId);

            // Добавляем задачу в эпик.
            await _projectManagmentRepository.IncludeTaskEpicAsync(epicId, projectTaskIdToNumbers);

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Задачи успешно добавлены в эпик.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotifySuccessIncludeEpicTask", userCode,
                UserConnectionModuleEnum.ProjectManagement);

            // TODO: Тут добавить запись активности пользователя по userId (кто добавил задачу в эпик).
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);

            await _hubNotificationService.Value.SendNotificationAsync("Что то пошло не так",
                "Ошибка при добавлении задач в эпик. Мы уже знаем о проблеме и уже занимаемся ей.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotifyErrorIncludeEpicTask", userCode,
                UserConnectionModuleEnum.ProjectManagement);

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
    public async Task PlaningSprintAsync(PlaningSprintInput planingSprintInput, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            List<long>? projectTaskIds = null;
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
                planingSprintInput.SprintStatus = (int)SprintStatusEnum.New;
                
                using var transactionScope = _transactionScopeFactory.CreateTransactionScope();
                var addedSprintId = await _projectManagmentRepository.PlaningSprintAsync(planingSprintInput, userId);
                var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
                
                // Добавляем задачи в спринт, если их включили в спринт.
                if (projectTaskIds is not null && projectTaskIds.Any())
                {
                    await _projectManagmentRepository.IncludeProjectTaskSprintAsync(projectTaskIds, addedSprintId);
                }

                if (planingSprintInput.DateStart.HasValue && planingSprintInput.DateEnd.HasValue)
                {
                    await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                        "Спринт успешно спланирован. Теперь его можно начать.",
                        NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotifySuccessPlaningSprint", userCode,
                        UserConnectionModuleEnum.ProjectManagement);
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

    /// <inheritdoc />
    public async Task<IEnumerable<WorkSpaceOutput>> GetWorkSpacesAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var result = await _projectManagmentRepository.GetWorkSpacesAsync(userId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RemoveProjectTaskAsync(long projectId, string projectTaskId, string account,
        TaskDetailTypeEnum taskType)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            // Проверяем, есть ли у пользователя роль на удаление задачи.
            var ifExistRoleRemoveTask = await _projectManagmentRoleRepository.Value
                .CheckProjectRoleAsync("ProjectRemoveTask", userId, projectId);
            
            if (!ifExistRoleRemoveTask)
            {
                var ex = new InvalidOperationException(
                    "У пользователя нет роли \"ProjectRemoveTask\" на удаление задачи. " +
                    $"UserId: {userId}. " +
                    $"ProjectId: {projectId}. " +
                    "Исключительная ситуация - пользователь не должен тогда был видеть кнопку удаления.");
                                                    
                await _discordService.SendNotificationErrorAsync(ex);

                throw ex;
            }

            // Получаем файлы задачи.
            var documents = (await _projectManagmentRepository.IfProjectTaskExistFileAsync(projectId,
                new[] { projectTaskId.GetProjectTaskIdFromPrefixLink() }))?.AsList();

            // Удаляем задачи и все связанные с ними данные.
            await _projectManagmentRepository.RemoveProjectTasksAsync(projectId, taskType,
                taskIds: new List<long> { projectTaskId.GetProjectTaskIdFromPrefixLink() }, documents);
            
            // Удаляем файлы задач на сервере, если они были.
            if (documents is not null && documents.Count > 0)
            {
                await _fileManagerService.Value.RemoveFilesAsync(documents);
            }
        }
        
        catch (Exception ex)
        {
             _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.
    
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

    /// <summary>
    /// Метод удаляет лишние статусы переходов.
    /// </summary>
    /// <param name="result">Результаты до чистки.</param>
    /// <param name="transitionType">Тип перехода.</param>
    /// <returns>Измененный список.</returns>
    /// <exception cref="InvalidOperationException">Может бахнуть, если что то пойдет не так.</exception>
    private async Task<List<AvailableTaskStatusTransitionOutput>> RemoveTransitionStatusesAsync(
        List<AvailableTaskStatusTransitionOutput> result, TransitionTypeEnum transitionType)
    {
        // Если есть оба системных названия, то оставим одно. InWork имеет приоритет.
        if (result.Find(x => x.StatusName.Equals("В работе")) is not null)
        {
            var removedDevelopment = result.Find(x => x.StatusName.Equals("В разработке"));

            if (removedDevelopment is not null)
            {
                result.Remove(removedDevelopment);
            }
        }
                
        if (result.Find(x => x.StatusName.Equals("В разработке")) is not null)
        {
            var removedDevelopment = result.Find(x => x.StatusName.Equals("В работе"));
                    
            if (removedDevelopment is not null)
            {
                result.Remove(removedDevelopment);
            }
        }
        
        // Статус "Новый" в приоритете.
        if (result.Find(x => x.StatusName.Equals("Новая")) is not null 
            && result.Find(x => x.StatusName.Equals("Новый")) is not null
            && transitionType == TransitionTypeEnum.Epic)
        {
            var removedDevelopment = result.Find(x => x.StatusName.Equals("Новая"));

            if (removedDevelopment is not null)
            {
                result.Remove(removedDevelopment);
            }
        }
        
        // Статус "Новая" в приоритете.
        if (result.Find(x => x.StatusName.Equals("Новая")) is not null 
            && result.Find(x => x.StatusName.Equals("Новый")) is not null
            && transitionType == TransitionTypeEnum.History)
        {
            var removedDevelopment = result.Find(x => x.StatusName.Equals("Новый"));

            if (removedDevelopment is not null)
            {
                result.Remove(removedDevelopment);
            }
        }

        // Если несколько системных названий Completed, то оставим одно.
        if (result.Count(x => x.StatusName.Equals("Completed")) > 1)
        {
            result = result.DistinctBy(d => d.StatusName).AsList();
        }

        return await Task.FromResult(result);
    }

    #endregion
}