using AutoMapper;
using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса спринтов.
/// </summary>
internal sealed class SprintService : ISprintService
{
    private readonly ILogger<SprintService> _logger;
    private readonly ISprintRepository _sprintRepository;
    private readonly IProjectManagementTemplateService _projectManagementTemplateService;
    private readonly IUserRepository _userRepository;
    private readonly IProjectSettingsConfigRepository _projectSettingsConfigRepository;
    private readonly IMapper _mapper;
    private readonly IProjectManagmentRepository _projectManagmentRepository;
    
    /// <summary>
    /// Конструктор.
    /// <param name="Логгер"></param>
    /// <param name="sprintRepository">Репозиторий спринтов.</param>
    /// <param name="projectManagementTemplateService">Сервис шаблонов проекта.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="projectManagmentRepository">Репозиторий модуля УП.</param>
    /// </summary>
    public SprintService(ILogger<SprintService> logger,
        ISprintRepository sprintRepository,
        IProjectManagementTemplateService projectManagementTemplateService,
         IUserRepository userRepository,
        IProjectSettingsConfigRepository projectSettingsConfigRepository,
        IMapper mapper,
         IProjectManagmentRepository projectManagmentRepository)
    {
        _logger = logger;
        _sprintRepository = sprintRepository;
        _projectManagementTemplateService = projectManagementTemplateService;
        _userRepository = userRepository;
        _projectSettingsConfigRepository = projectSettingsConfigRepository;
        _mapper = mapper;
        _projectManagmentRepository = projectManagmentRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TaskSprintExtendedOutput>> GetSprintsAsync(long projectId)
    {
        try
        {
            var result = await _sprintRepository.GetSprintsAsync(projectId);

            return result ?? Enumerable.Empty<TaskSprintExtendedOutput>();
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<TaskSprintExtendedOutput> GetSprintAsync(long projectSprintId, long projectId, string account)
    {
        try
        {
            var result = await _sprintRepository.GetSprintAsync(projectSprintId, projectId);

            if (result is null)
            {
                throw new InvalidOperationException("Не удалось получить детали спринта. " +
                                                    $"ProjectSprintId: {projectSprintId}. " +
                                                    $"ProjectId: {projectId}.");
            }
            
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
            
            // Получаем задачи спринта, если они есть.
            // Это могут быть задачи, ошибки, истории, эпики - все, что может входить в спринт.
            var sprintTasks = await _sprintRepository.GetProjectSprintTasksAsync(projectId, projectSprintId, strategy);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}