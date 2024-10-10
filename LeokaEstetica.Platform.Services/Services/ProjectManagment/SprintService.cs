using System.Globalization;
using AutoMapper;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Dto.Base.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Services.Helpers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса спринтов.
/// </summary>
internal sealed class SprintService : ISprintService
{
    private readonly ILogger<SprintService>? _logger;
    private readonly ISprintRepository _sprintRepository;
    private readonly IProjectManagementTemplateService _projectManagementTemplateService;
    private readonly IUserRepository _userRepository;
    private readonly IProjectSettingsConfigRepository _projectSettingsConfigRepository;
    private readonly IMapper _mapper;
    private readonly IProjectManagmentRepository _projectManagmentRepository;
    private readonly Lazy<IDistributionStatusTaskService> _distributionStatusTaskService;
    private readonly IDiscordService _discordService;
    private readonly IProjectManagementSettingsRepository _projectManagementSettingsRepository;

    /// <summary>
    /// Список недопустимых для начала спринта статусов.
    /// </summary>
    private readonly List<SprintStatusEnum> _notAvailableStartSprintStatuses = new()
    {
        SprintStatusEnum.InWork,
        SprintStatusEnum.Completed,
        SprintStatusEnum.Closed
    };
    
    private readonly Lazy<IHubNotificationService> _hubNotificationService;

    /// <summary>
    /// Конструктор.
    /// <param name="Логгер"></param>
    /// <param name="sprintRepository">Репозиторий спринтов.</param>
    /// <param name="projectManagementTemplateService">Сервис шаблонов проекта.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="projectManagmentRepository">Репозиторий модуля УП.</param>
    /// <param name="distributionStatusTaskService">Сервис распределения по статусам.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="projectManagementSettingsRepository">Репозиторий настроек проекта.</param>
    /// <param name="hubNotificationService">Сервис уведомлений хабов.</param>
    /// </summary>
    public SprintService(ILogger<SprintService>? logger,
        ISprintRepository sprintRepository,
        IProjectManagementTemplateService projectManagementTemplateService,
        IUserRepository userRepository,
        IProjectSettingsConfigRepository projectSettingsConfigRepository,
        IMapper mapper,
        IProjectManagmentRepository projectManagmentRepository,
        Lazy<IDistributionStatusTaskService> distributionStatusTaskService,
        IDiscordService discordService,
        IProjectManagementSettingsRepository projectManagementSettingsRepository,
        Lazy<IHubNotificationService> hubNotificationService)
    {
        _logger = logger;
        _sprintRepository = sprintRepository;
        _projectManagementTemplateService = projectManagementTemplateService;
        _userRepository = userRepository;
        _projectSettingsConfigRepository = projectSettingsConfigRepository;
        _mapper = mapper;
        _projectManagmentRepository = projectManagmentRepository;
        _distributionStatusTaskService = distributionStatusTaskService;
        _discordService = discordService;
        _projectManagementSettingsRepository = projectManagementSettingsRepository;
        _hubNotificationService = hubNotificationService;
    }

    #region Публичные методы

    /// <inheritdoc />
    public async Task<TaskSprintListResult> GetSprintsAsync(long projectId)
    {
        try
        {
            var result = await _sprintRepository.GetSprintsAsync(projectId);

            var sprintsNew = new List<TaskSprintExtendedOutput>(result
                .Count(s => s.SprintStatusId == (int)SprintStatusEnum.New));

            var sprintsInWork = new List<TaskSprintExtendedOutput>(result
                .Count(s => s.SprintStatusId == (int)SprintStatusEnum.InWork));

            var sprintsCompleted = new List<TaskSprintExtendedOutput>(result
                .Count(s => s.SprintStatusId == (int)SprintStatusEnum.Completed));

            foreach (var sprint in result)
            {
                switch (sprint.SprintStatusId)
                {
                    case (int)SprintStatusEnum.New:
                        sprintsNew.Add(sprint);
                        break;

                    case (int)SprintStatusEnum.InWork:
                        sprintsInWork.Add(sprint);
                        break;

                    case (int)SprintStatusEnum.Completed:
                        sprintsCompleted.Add(sprint);
                        break;
                }
            }

            var sprints = new TaskSprintListResult
            {
                SprintsNew = sprintsNew.OrderByDescending(s => s.CreatedAt),
                SprintsInWork = sprintsInWork.OrderByDescending(s => s.CreatedAt),
                SprintsCompleted = sprintsCompleted.OrderByDescending(s => s.CreatedAt)
            };

            return sprints;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<TaskSprintExtendedOutput> GetSprintAsync(long projectSprintId, long projectId, string account)
    {
        try
        {
            // Получаем данные спринта.
            var result = await _sprintRepository.GetSprintAsync(projectSprintId, projectId);

            if (result is null)
            {
                throw new InvalidOperationException("Не удалось получить детали спринта. " +
                                                    $"ProjectSprintId: {projectSprintId}. " +
                                                    $"ProjectId: {projectId}.");
            }
            
            // Заполняем доп.поля деталей спринта.
            await ModificateSprintDetailsAsync(result);
            
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

            if (projectSettingsItems is null || projectSettingsItems.Count == 0)
            {
                throw new InvalidOperationException("Ошибка получения настроек проекта. " +
                                                    $"ProjectId: {projectId}. " +
                                                    $"UserId: {userId}");
            }

            var template = projectSettingsItems.Find(x =>
                x.ParamKey.Equals(GlobalConfigKeys.ConfigSpaceSetting.PROJECT_MANAGEMENT_TEMPLATE_ID));
            var templateId = Convert.ToInt32(template!.ParamValue);

            // Получаем набор статусов, которые входят в выбранный шаблон.
            var items = await _projectManagementTemplateService.GetProjectManagmentTemplatesAsync(templateId);
            var templateStatusesItems = _mapper.Map<IEnumerable<ProjectManagmentTaskTemplateResult>>(items);
            var statuses = templateStatusesItems?.AsList();

            if (statuses is null || statuses.Count == 0)
            {
                throw new InvalidOperationException("Не удалось получить набор статусов шаблона." +
                                                    $" TemplateId: {templateId}." +
                                                    $"ProjectId: {projectId}.");
            }

            // Проставляем Id шаблона статусам.
            await _projectManagementTemplateService.SetProjectManagmentTemplateIdsAsync(statuses);

            // Получаем задачи спринта.
            result.SprintTasks = await GetSprintTasksAsync(statuses, templateId, projectId, userId, projectSprintId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateSprintNameAsync(long projectSprintId, long projectId, string sprintName, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            await _sprintRepository.UpdateSprintNameAsync(projectSprintId, projectId, sprintName);

            // TODO: Добавить запись активности (кто изменил название спринта).
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateSprintDetailsAsync(long projectSprintId, long projectId, string sprintDetails,
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

            await _sprintRepository.UpdateSprintDetailsAsync(projectSprintId, projectId, sprintDetails);

            // TODO: Добавить запись активности (кто изменил описание спринта).
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task InsertOrUpdateSprintExecutorAsync(long projectSprintId, long projectId, long executorId,
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
            
            await _sprintRepository.InsertOrUpdateSprintExecutorAsync(projectSprintId, projectId, executorId);
            
            // TODO: Добавить запись активности (кто назначил/обновил исполнителя спринта).
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task InsertOrUpdateSprintWatchersAsync(long projectSprintId, long projectId,
        IEnumerable<long> watcherIds, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            await _sprintRepository.InsertOrUpdateSprintWatchersAsync(projectSprintId, projectId, watcherIds);
            
            // TODO: Добавить запись активности (кто назначил/обновил наблюдателей спринта).
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task StartSprintAsync(long projectSprintId, long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            // Ищем уже активный спринт проекта.
            var activeSprint = await _sprintRepository.CheckActiveSprintAsync(projectId);
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            // Нельзя начать спринт проекта, если уже есть активный спринт у проекта. 
            if (activeSprint)
            {
                var ex = new InvalidOperationException("У проекта уже имеется запущенный спринт. " +
                                                       "Начать новый невозможно. " +
                                                       $"ProjectSprintId: {projectSprintId}. " +
                                                       $"ProjectId: {projectId}.");
                await _discordService.SendNotificationErrorAsync(ex);
                
                _logger?.LogError(ex, ex.Message);

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    "У проекта уже имеется запущенный спринт.", NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING,
                    "SendNotificationWarningStartSprint", userCode, UserConnectionModuleEnum.ProjectManagement);

                return;
            }

            // Получаем данные спринта.
            var sprint = await GetSprintByProjectSprintIdByProjectIdAsync(projectSprintId, projectId, userCode);

            // К этому моменту мы уже показали пользователю уведомление об этом, не ломаем приложение.
            if (sprint is null)
            {
                return;
            }
            
            // Проверяем даты на корректность.
            if (!DateTime.TryParse(sprint.DateStart, out var dateStart) || 
                !DateTime.TryParse(sprint.DateEnd, out var dateEnd))
            {
                // Нельзя начать спринт - даты не заполнены.
                var ex = new InvalidOperationException(
                    "Нельзя начать спринт - даты (начала и окончания) не заполнены. " +
                    $"ProjectSprintId: {projectSprintId}. " +
                    $"ProjectId: {projectId}.");
                await _discordService.SendNotificationErrorAsync(ex);
                
                _logger?.LogError(ex, ex.Message);

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    "Нельзя начать спринт - даты (начала и окончания) не заполнены.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningStartSprint", userCode,
                    UserConnectionModuleEnum.ProjectManagement);

                return;
            }
            
            // Если дата начала в прошлом, то скорректируем от сегодня.
            if (dateStart < DateTime.UtcNow)
            {
                var setting = await _projectManagementSettingsRepository.GetProjectSprintDurationSettingsAsync(
                    projectId);

                if (setting is null)
                {
                    var ex = new InvalidOperationException(
                        $"Ошибка получения длительности спринтов проекта {projectId}");
                    await _discordService.SendNotificationErrorAsync(ex);
                
                    throw ex;
                }
                
                // Корректируем длительность спринта в соответствии с настройками.
                if (setting.SysName!.Equals("OneWeek"))
                {
                    sprint.DateStart = DateTime.UtcNow.ToString(CultureInfo.CurrentCulture);
                    
                    // Длительность спринта 1 неделя.
                    sprint.DateEnd = dateStart.AddDays(7).ToString(CultureInfo.CurrentCulture);
                }
                
                if (setting.SysName!.Equals("TwoWeek"))
                {
                    sprint.DateStart = DateTime.UtcNow.ToString(CultureInfo.CurrentCulture);
                    
                    // Длительность спринта 2 недели.
                    sprint.DateEnd = dateStart.AddDays(14).ToString(CultureInfo.CurrentCulture);
                }
            }

            if (_notAvailableStartSprintStatuses.Contains((SprintStatusEnum)sprint.SprintStatusId))
            {
                // Обнаружен недопустимый для старта спринта статус.
                var ex = new InvalidOperationException(
                    "Обнаружен недопустимый для старта спринта статус. " +
                    $"ProjectSprintId: {projectSprintId}. " +
                    $"ProjectId: {projectId}. " +
                    $"SprintStatus: {((SprintStatusEnum)sprint.SprintStatusId).ToString()}.");
                await _discordService.SendNotificationErrorAsync(ex);
                
                _logger?.LogError(ex, ex.Message);

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    "Невозможно начать спринт в статусе: " +
                    $"{((SprintStatusEnum)sprint.SprintStatusId).GetEnumDescription()}. " +
                    $"Спринт должен находится в статусе: {SprintStatusEnum.New.GetEnumDescription()}",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningStartSprint", userCode,
                    UserConnectionModuleEnum.ProjectManagement);

                return;
            }
            
            // Проверяем, имеет ли спринт задачи.
            var sprintTaskCount = await _sprintRepository.GetCountSprintTasksAsync(projectSprintId, projectId);
            
            if (sprintTaskCount == 0)
            {
                // Нельзя начать пустой спринт.
                var ex = new InvalidOperationException(
                    "Нельзя начать пустой спринт. " +
                    $"ProjectSprintId: {projectSprintId}. " +
                    $"ProjectId: {projectId}. " +
                    $"SprintStatus: {((SprintStatusEnum)sprint.SprintStatusId).ToString()}.");
                await _discordService.SendNotificationErrorAsync(ex);
                
                _logger?.LogError(ex, ex.Message);

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    "Нельзя начать пустой спринт.", NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING,
                    "SendNotificationWarningStartSprint", userCode, UserConnectionModuleEnum.ProjectManagement);

                return;
            }
            
            // Запускаем спринт проекта.
            await _sprintRepository.RunSprintAsync(projectSprintId, projectId);

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                $"Спринт \"{sprint.SprintName}\" успешно начат.", NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS,
                "SendNotificationSuccessStartSprint", userCode, UserConnectionModuleEnum.ProjectManagement);

            // TODO: Добавить запись активности (кто начал спринт).
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ManualCompleteSprintOutput> ManualCompleteSprintAsync(ManualCompleteSprintInput sprintInput,
        string account)
    {
        try
        {
            var result = new ManualCompleteSprintOutput
            {
                ProjectId = sprintInput.ProjectId,
                ProjectSprintId = sprintInput.ProjectSprintId
            };
            
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);
            
            // Получаем данные спринта.
            var sprint = await GetSprintByProjectSprintIdByProjectIdAsync(sprintInput.ProjectSprintId,
                sprintInput.ProjectId, userCode);
                
            // К этому моменту мы уже показали пользователю уведомление об этом, не ломаем приложение.
            if (sprint is null)
            {
                return result;
            }

            // Если спринт не был запущен.
            if (sprint.SprintStatusId != (int)SprintStatusEnum.InWork)
            {
                var ex = new InvalidOperationException(
                    "Нельзя остановить спринт, который не был запущен. " +
                    $"ProjectSprintId: {sprintInput.ProjectSprintId}. " +
                    $"ProjectId: {sprintInput.ProjectId}. " +
                    $"SprintStatus: {((SprintStatusEnum)sprint.SprintStatusId).ToString()}");
                await _discordService.SendNotificationErrorAsync(ex);

                _logger?.LogError(ex, ex.Message);

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    "Нельзя остановить спринт, который не был запущен.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningStartSprint", userCode,
                    UserConnectionModuleEnum.ProjectManagement);

                return result;
            }
            
            // Если действие еще не обрабатывалось.
            if (!sprintInput.IsProcessedAction)
            {
                // TODO: Когда будет реализовано создание кастомных статусов, то сопосатвялть еще таблицу кастомных
                // TODO: статусов пользователей из таблицы в задаче #31632122 (Kaiten).
                // Проверяем, есть ли незавершенные задачи спринта.
                // Ищем по статусу (через сопоставление с системными статусами).
                var notCompletedSprintTasks = (await _sprintRepository.GetNotCompletedSprintTasksAsync(
                    sprintInput.ProjectSprintId, sprintInput.ProjectId))?.AsList();

                // Если у спринта есть незавершенные задачи, то не завершаем спринт, а требуем от пользователя действия.
                if (notCompletedSprintTasks is not null && notCompletedSprintTasks.Count > 0)
                {
                    result.NotCompletedSprintTaskIds = notCompletedSprintTasks;
                    result.NeedSprintAction = new BaseNeedSprintAction(NeedSprintActionTypeEnum.NotCompletedTask)
                    {
                        IsNeedUserAction = true
                    };
                    result.IsProcessedAction = false;

                    return result;
                }
                
                // Если нет незавершенных заадч у спринта, то можем сразу его завершить.
                if (notCompletedSprintTasks is null || notCompletedSprintTasks.Count == 0)
                {
                    // Завершаем спринт.
                    await _sprintRepository.ManualCompleteSprintAsync(sprintInput.ProjectSprintId,
                        sprintInput.ProjectId);
                
                    return result;
                }
            }

            // Если действие уже было выбрано пользователем, то обрабатываем его и завершаем спринт.
            if (sprintInput.IsProcessedAction)
            {
                if (sprintInput.NotCompletedSprintTaskIds is null || !sprintInput.NotCompletedSprintTaskIds.Any())
                {
                    throw new InvalidOperationException(
                        $"Нет незавершенных задач. {JsonConvert.SerializeObject(sprintInput)}.");
                }
                
                result.NotCompletedSprintTaskIds = sprintInput.NotCompletedSprintTaskIds;

                // Находим вариант, который выбрал пользователь.
                var selectedVariant = sprintInput.NeedSprintAction!.ActionVariants?.FirstOrDefault(x => x.IsSelected);

                if (selectedVariant is null)
                {
                    throw new InvalidOperationException(
                        "Не удалось получить выбранный вариант при завершении спринта. " +
                        $"SprintData: {JsonConvert.SerializeObject(sprintInput)}");
                }

                switch (selectedVariant.VariantSysName)
                {
                    // Если бэклог, то помечаем задачи тегом, чтобы выделить их в раб.пространстве.
                    case "Backlog":
                        break;

                    // Если в один из будущих спринтов.
                    case "Prospective":
                        if (!sprintInput.MoveSprintId.HasValue)
                        {
                            // Один из спринтов должен был быть выбран пользователем.
                            await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                                "Не выбран спринт для переноса незавершенных задач.",
                                NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING,
                                "SendNotificationSuccessStartSprint", userCode,
                                UserConnectionModuleEnum.ProjectManagement);

                            return result;
                        }
                        
                        // Переносим незавершенные задачи в указанный спринт.
                        await _sprintRepository.MoveSprintTasksAsync(sprintInput.MoveSprintId.Value,
                            sprintInput.NotCompletedSprintTaskIds);

                        break;
                    
                    // Если в новый спринт.
                    case "NewSprint":
                        if (string.IsNullOrWhiteSpace(sprintInput.MoveSprintName))
                        {
                            // Название нового спринта должно быть заполнено.
                            await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                                "Не выбран спринт для переноса незавершенных задач.",
                                NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING,
                                "SendNotificationSuccessStartSprint", userCode,
                                UserConnectionModuleEnum.ProjectManagement);

                            return result;
                        }
                        
                        // Планируем новый спринт и перемещаем в него незавершенные задачи.
                        await _sprintRepository.PlaningNewSprintAndMoveNotCompletedSprintTasksAsync(
                            sprintInput.ProjectId, sprintInput.NotCompletedSprintTaskIds, sprintInput.MoveSprintName);
                        
                        break;
                }
                
                // Завершаем спринт.
                await _sprintRepository.ManualCompleteSprintAsync(sprintInput.ProjectSprintId, sprintInput.ProjectId);
            }

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                $"Спринт \"{sprint.SprintName}\" успешно завершен.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessStartSprint", userCode,
                UserConnectionModuleEnum.ProjectManagement);

            // TODO: Добавить запись активности (кто вручную завершил спринт).

            return result;
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TaskSprintExtendedOutput>> GetAvailableNextSprintsAsync(long projectSprintId,
        long projectId)
    {
        try
        {
            var result = await _sprintRepository.GetAvailableNextSprintsAsync(projectSprintId, projectId);

            return result;
        }
        
        catch (Exception ex)
        {
             _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task ExcludeSprintTasksAsync(long sprintId, IEnumerable<string>? sprintTaskIds, string account)
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
            await _sprintRepository.ExcludeSprintTasksAsync(sprintId,
                sprintTaskIds!.Select(x => x.GetProjectTaskIdFromPrefixLink()));

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Задача успешно исключена из спринта.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotifySuccessExcludeSprintTask",
                userCode, UserConnectionModuleEnum.ProjectManagement);
        }
        
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);

            await _hubNotificationService.Value.SendNotificationAsync("Что то пошло не так",
                "Ошибка при исключении задачи из спринта.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotifyErrorExcludeSprintTask", userCode,
                UserConnectionModuleEnum.ProjectManagement);

            throw;
        }
    }

    /// <inheritdoc />
    public async Task IncludeSprintTasksAsync(long sprintId, IEnumerable<string>? sprintTaskIds, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            await _sprintRepository.IncludeSprintTasksAsync(sprintId,
                sprintTaskIds!.Select(x => x.GetProjectTaskIdFromPrefixLink()));

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                 "Задача успешно включена в спринт.",
                 NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotifySuccessProjectTaskIncludeSprint",
                 userCode, UserConnectionModuleEnum.ProjectManagement);
        }

        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RemoveSprintAsync(long sprintId, long projectSprintId)
    {
        try
        {
            await _sprintRepository.RemoveSprintAsync(sprintId, projectSprintId);
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
    /// Метод заполняет доп.поля деталей спринта.
    /// </summary>
    /// <param name="sprintData">Данные спринта до модификации.</param>
    private async Task ModificateSprintDetailsAsync(TaskSprintExtendedOutput sprintData)
    {
        // Заполняем название исполнителя, если он задан у спринта.
        if (sprintData.ExecutorId.HasValue)
        {
            var executors = await _userRepository.GetExecutorNamesByExecutorIdsAsync(
                new[] { sprintData.ExecutorId.Value });

            if (executors.TryGet(sprintData.ExecutorId.Value) is not null)
            {
                sprintData.ExecutorName = executors.TryGet(sprintData.ExecutorId.Value)?.FullName.Trim();
            }
        }
        
        // Заполняем наблюдателей, если они заданы у спринта.
        if (sprintData.WatcherIds is not null && sprintData.WatcherIds.Any())
        {
            var watchers = await _userRepository.GetWatcherNamesByWatcherIdsAsync(sprintData.WatcherIds);
            
            // Названия наблюдателей задачи.
            if (watchers is not null && watchers.Count > 0)
            {
                foreach (var w in sprintData.WatcherIds)
                {
                    var watcher = watchers.TryGet(w)?.FullName;
                            
                    // Если такое бахнуло, то не добавляем в список, но и не ломаем приложение.
                    // Просто логируем такое.
                    if (watcher is null)
                    {
                        var ex = new InvalidOperationException("Обнаружен наблюдатель с NULL. " +
                                                               $"WatcherId: {w}");
                        await _discordService.SendNotificationErrorAsync(ex);
                        _logger?.LogError(ex, ex.Message);
                                
                        continue;
                    }

                    if (sprintData.WatcherNames is null)
                    {
                        sprintData.WatcherNames = new List<string>();   
                    }

                    sprintData.WatcherNames.Add(watcher);
                }
            }
        }
        
        // Заполняем автора (кто создал спринт).
        var authors = await _userRepository.GetAuthorNamesByAuthorIdsAsync(new [] { sprintData.CreatedBy });
        
        if (authors.Count == 0)
        {
            throw new InvalidOperationException("Не удалось получить авторов задач.");
        }
        
        sprintData.AuthorName = authors.TryGet(sprintData.CreatedBy)?.FullName;
    }

    /// <summary>
    /// Метод получает данные спринта.
    /// </summary>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userCode">Код пользователя.</param>
    /// <returns>Данные спринта.</returns>
    private async Task<TaskSprintExtendedOutput?> GetSprintByProjectSprintIdByProjectIdAsync(long projectSprintId,
        long projectId, Guid userCode)
    {
        // Получаем данные спринта.
        var sprint = await _sprintRepository.GetSprintAsync(projectSprintId, projectId);

        if (sprint is null)
        {
            var ex = new InvalidOperationException("Не удалось получить данные спринта. " +
                                                   $"ProjectSprintId: {projectSprintId}. " +
                                                   $"ProjectId: {projectId}.");
            await _discordService.SendNotificationErrorAsync(ex);
                
            _logger?.LogError(ex, ex.Message);

            await _hubNotificationService.Value.SendNotificationAsync("Что то пошло не так",
                "Ошибка при получении данных спринта. Мы уже знаем о проблеме и уже занимаемся ей.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationWarningStartSprint", userCode,
                UserConnectionModuleEnum.ProjectManagement);

            return null;
        }

        return sprint;
    }

    /// <summary>
    /// Метод получает задачи спринта и наполняет результат спринта.
    /// </summary>
    /// <param name="statuses">Список статусов.</param>
    /// <param name="templateId">Id шаблона проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectSprintId">Id спринта в рамках проекта.</param>
    /// <returns>Список задач спринта.</returns>
    private async Task<IEnumerable<ProjectManagmentTaskOutput>> GetSprintTasksAsync(
        List<ProjectManagmentTaskTemplateResult> statuses, int templateId, long projectId, long userId,
        long projectSprintId)
    {
        // Получаем выбранную пользователем стратегию представления.
        var strategy = await _projectManagmentRepository.GetProjectUserStrategyAsync(projectId, userId);
            
        // Добавляем в результат статусы.
        var projectManagmentTaskStatuses = statuses.First().ProjectManagmentTaskStatusTemplates.AsList();

        if (projectManagmentTaskStatuses.Count == 0)
        {
            return Enumerable.Empty<ProjectManagmentTaskOutput>();
        }

        // Получаем задачи спринта, если они есть.
        // Это могут быть задачи, ошибки, истории, эпики - все, что может входить в спринт.
        var sprintTasks = (await _sprintRepository.GetProjectSprintTasksAsync(projectId, projectSprintId,
            strategy!, templateId))?.OrderBy(o => o.Created).AsList();
        
        if (sprintTasks is null || sprintTasks.Count == 0)
        {
            return Enumerable.Empty<ProjectManagmentTaskOutput>();
        }
        
        // Заполняем статусы задачами.
        await _distributionStatusTaskService.Value.DistributionStatusTaskAsync(projectManagmentTaskStatuses,
            sprintTasks, ModifyTaskStatuseTypeEnum.Sprint, projectId, null, strategy!);

        // Заполняем задачи спринта.
        var mapSprintTasks = _mapper.Map<List<ProjectManagmentTaskOutput>>(sprintTasks.OrderBy(o => o.Created));
        
        // Готовим списки задач для заполнения словарей.
        var taskAndErrorIds = new List<long>();
        var storyIds = new List<long>();
        var epicIds = new List<long>();

        // Если есть задачи с типом "Задача" или "Ошибка".
        if (mapSprintTasks.Any(x =>
                new[]
                    {
                        (int)ProjectTaskTypeEnum.Task,
                        (int)ProjectTaskTypeEnum.Error
                    }
                    .Contains(x.TaskTypeId)))
        {
            taskAndErrorIds.AddRange(mapSprintTasks
                .Where(x => new[]
                {
                    (int)ProjectTaskTypeEnum.Task,
                    (int)ProjectTaskTypeEnum.Error
                }.Contains(x.TaskTypeId))
                .Select(x => x.ProjectTaskId));
        }

        // Если есть задачи с типом "История".
        if (mapSprintTasks.Any(x => x.TaskTypeId == (int)ProjectTaskTypeEnum.Story))
        {
            storyIds.AddRange(mapSprintTasks
                .Where(x => x.TaskTypeId == (int)ProjectTaskTypeEnum.Story)
                .Select(x => x.ProjectTaskId));
        }

        // Если есть задачи с типом "Эпик".
        if (mapSprintTasks.Any(x => x.TaskTypeId == (int)ProjectTaskTypeEnum.Epic))
        {
            epicIds.AddRange(mapSprintTasks
                .Where(x => x.TaskTypeId == (int)ProjectTaskTypeEnum.Epic)
                .Select(x => x.ProjectTaskId));
        }

        IDictionary<long, ProjectTaskTypeOutput>? taskTypeAndErrorDict = null;
        IDictionary<int, TaskPriorityOutput>? taskPriorityNamesDict = null;

        // Заполняем словарь задач и ошибок.
        if (taskAndErrorIds.Count > 0)
        {
            taskTypeAndErrorDict = await _projectManagmentRepository.GetProjectTaskStatusesAsync(projectId,
                taskAndErrorIds);

            taskPriorityNamesDict = await _projectManagmentRepository.GetPriorityNamesByPriorityIdsAsync(
                    mapSprintTasks.Select(x => x.PriorityId));
        }
        
        IDictionary<long, ProjectTaskTypeOutput>? storyTypeDict = null;
        IDictionary<long, ProjectTaskTypeOutput>? epicTypeDict = null;

        // Заполняем словарь историй.
        if (storyIds.Count > 0)
        {
            storyTypeDict = await _projectManagmentRepository.GetProjectStoryStatusesAsync(projectId, storyIds);
        }
        
        // Заполняем словарь эпиков.
        if (epicIds.Count > 0)
        {
            epicTypeDict = await _projectManagmentRepository.GetProjectEpicStatusesAsync(projectId, epicIds);
        }
        
        // Получаем имена исполнителей задач.
        var executors = await _userRepository.GetExecutorNamesByExecutorIdsAsync(
            mapSprintTasks.Select(x => x.ExecutorId));

        // Заполняем задачи спринта доп.полями.
        foreach (var st in mapSprintTasks)
        {
            // Заполняем исполнителей задач спринта.
            st.Executor ??= new Executor();
            st.Executor.ExecutorName = executors.TryGet(st.ExecutorId)?.FullName;
            
            // Заполняем статус задач спринта.
            if (st.TaskTypeId is (int)ProjectTaskTypeEnum.Task or (int)ProjectTaskTypeEnum.Error)
            {
                st.TaskStatusName = taskTypeAndErrorDict?.TryGet(st.ProjectTaskId)?.TaskStatusName;
                st.TaskTypeName = taskTypeAndErrorDict?.TryGet(st.ProjectTaskId)?.TaskTypeName;
            }
            
            if (st.TaskTypeId == (int)ProjectTaskTypeEnum.Story)
            {
                st.TaskStatusName = storyTypeDict?.TryGet(st.ProjectTaskId)?.TaskStatusName;
                st.TaskTypeName = storyTypeDict?.TryGet(st.ProjectTaskId)?.TaskTypeName;
            }
            
            if (st.TaskTypeId == (int)ProjectTaskTypeEnum.Epic)
            {
                st.TaskStatusName = epicTypeDict?.TryGet(st.ProjectTaskId)?.TaskStatusName;
                st.TaskTypeName = epicTypeDict?.TryGet(st.ProjectTaskId)?.TaskTypeName;
            }

            st.PriorityName = taskPriorityNamesDict?.TryGet(st.PriorityId)?.PriorityName;
        }

        return mapSprintTasks;
    }

    #endregion
}