using System.Globalization;
using AutoMapper;
using LeokaEstetica.Platform.Access.Abstractions.AvailableLimits;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Helpers;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Notification;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Finder.Chains.Project;
using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectTeam;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.ProjectTeam;
using LeokaEstetica.Platform.Models.Entities.User;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Moderation.Abstractions.Vacancy;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using LeokaEstetica.Platform.Services.Abstractions.Vacancy;
using LeokaEstetica.Platform.Services.Builders;
using LeokaEstetica.Platform.Services.Consts;
using LeokaEstetica.Platform.Services.Strategies.Project.Team;

namespace LeokaEstetica.Platform.Services.Services.Project;

/// <summary>
/// Класс реализует методы сервиса проектов.
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogService _logService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IProjectNotificationsService _projectNotificationsService;
    private readonly IVacancyService _vacancyService;
    private readonly IVacancyRepository _vacancyRepository;

    // Определяем всю цепочку фильтров.
    private readonly BaseProjectsFilterChain _dateProjectsFilterChain = new DateProjectsFilterChain();
    private readonly BaseProjectsFilterChain _projectsVacanciesFilterChain = new ProjectsVacanciesFilterChain();
    private readonly BaseProjectsFilterChain _projectStageConceptFilterChain = new ProjectStageConceptFilterChain();

    private readonly BaseProjectsFilterChain _projectStageSearchTeamFilterChain =
        new ProjectStageSearchTeamFilterChain();

    private readonly BaseProjectsFilterChain _projectStageTestingFilterChain = new ProjectStageTestingFilterChain();

    private readonly BaseProjectsFilterChain _projectStageDevelopmentFilterChain =
        new ProjectStageDevelopmentFilterChain();

    private readonly BaseProjectsFilterChain _projectStageProjectingFilterChain =
        new ProjectStageProjectingFilterChain();

    private readonly BaseProjectsFilterChain _projectStageSupportFilterChain = new ProjectStageSupportFilterChain();

    private readonly BaseProjectsFilterChain _projectStageStartFilterChain = new ProjectStageStartFilterChain();

    private readonly BaseProjectsFilterChain _projectStageSearchInvestorsFilterChain =
        new ProjectStageSearchInvestorsFilterChain();

    private readonly IAvailableLimitsService _availableLimitsService;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IFareRuleRepository _fareRuleRepository;

    /// <summary>
    /// Список названий тарифов, которые дают выделение цветом.
    /// </summary>
    private static readonly List<string> _fareRuleTypesNames = new()
    {
        FareRuleTypeEnum.Business.GetEnumDescription(),
        FareRuleTypeEnum.Professional.GetEnumDescription()
    };

    /// <summary>
    /// Список типов приглашений в проект.
    /// </summary>
    private static readonly List<ProjectInviteTypeEnum> _projectInviteTypes = new()
    {
        ProjectInviteTypeEnum.Email,
        ProjectInviteTypeEnum.Login,
        ProjectInviteTypeEnum.PhoneNumber,
        ProjectInviteTypeEnum.Link
    };

    private readonly IVacancyModerationService _vacancyModerationService;
    private static readonly string _approveVacancy = "Опубликована";

    private readonly IProjectNotificationsRepository _projectNotificationsRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    /// <param name="logService">Сервис логера.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="projectNotificationsService">Сервис уведомлений.</param>
    /// <param name="vacancyService">Сервис вакансий.</param>
    /// <param name="vacancyRepository">Репозиторий вакансий.</param>
    /// <param name="availableLimitsService">Сервис проверки лимитов.</param>
    /// <param name="vacancyModerationService">Сервис модерации вакансий проектов.</param>
    /// <param name="notificationsRepository">Репозиторий уведомлений.</param>
    public ProjectService(IProjectRepository projectRepository,
        ILogService logService,
        IUserRepository userRepository,
        IMapper mapper,
        IProjectNotificationsService projectNotificationsService,
        IVacancyService vacancyService,
        IVacancyRepository vacancyRepository, 
        IAvailableLimitsService availableLimitsService, 
        ISubscriptionRepository subscriptionRepository, 
        IFareRuleRepository fareRuleRepository, 
        IVacancyModerationService vacancyModerationService, 
        IProjectNotificationsRepository projectNotificationsRepository)
    {
        _projectRepository = projectRepository;
        _logService = logService;
        _userRepository = userRepository;
        _mapper = mapper;
        _projectNotificationsService = projectNotificationsService;
        _vacancyService = vacancyService;
        _vacancyRepository = vacancyRepository;
        _availableLimitsService = availableLimitsService;
        _subscriptionRepository = subscriptionRepository;
        _fareRuleRepository = fareRuleRepository;
        _vacancyModerationService = vacancyModerationService;
        _projectNotificationsRepository = projectNotificationsRepository;

        // Определяем обработчики цепочки фильтров.
        _dateProjectsFilterChain.Successor = _projectsVacanciesFilterChain;
        _projectsVacanciesFilterChain.Successor = _projectStageConceptFilterChain;
        _projectStageConceptFilterChain.Successor = _projectStageSearchTeamFilterChain;
        _projectStageSearchTeamFilterChain.Successor = _projectStageTestingFilterChain;
        _projectStageTestingFilterChain.Successor = _projectStageDevelopmentFilterChain;
        _projectStageDevelopmentFilterChain.Successor = _projectStageProjectingFilterChain;
        _projectStageProjectingFilterChain.Successor = _projectStageSupportFilterChain;
        _projectStageSupportFilterChain.Successor = _projectStageStartFilterChain;
        _projectStageStartFilterChain.Successor = _projectStageSearchInvestorsFilterChain;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод создает новый проект пользователя.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="projectStage">Стадия проекта.</param>
    /// <returns>Данные нового проекта.</returns>
    public async Task<UserProjectEntity> CreateProjectAsync(string projectName, string projectDetails, string account,
        ProjectStageEnum projectStage)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            // Получаем подписку пользователя.
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(userId);
            
            // Получаем тариф, на который оформлена подписка у пользователя.
            var fareRule = await _fareRuleRepository.GetByIdAsync(userSubscription.ObjectId);
            
            // Проверяем доступо ли пользователю создание проекта.
            var availableCreateProjectLimit =
                await _availableLimitsService.CheckAvailableCreateProjectAsync(userId, fareRule.Name);

            // Если лимит по тарифу превышен.
            if (!availableCreateProjectLimit)
            {
                var ex = new Exception($"Превышен лимит проектов по тарифу. UserId: {userId}. Тариф: {fareRule.Name}");
                await _logService.LogErrorAsync(ex);
                await _projectNotificationsService.SendNotificationWarningLimitFareRuleProjectsAsync("Что то пошло не так",
                    "Превышен лимит проектов по тарифу.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, userId);

                return null;
            }

            // Проверяем существование такого проекта у текущего пользователя.
            var isCreatedProject = await _projectRepository.CheckCreatedProjectByProjectNameAsync(projectName, userId);

            // Есть дубликат, нельзя создать проект.
            if (isCreatedProject)
            {
                await _projectNotificationsService
                    .SendNotificationWarningDublicateUserProjectAsync("Увы...", "Такой проект у вас уже существует.",
                        NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, userId);

                return null;
            }

            var statusName = ProjectStatus.GetProjectStatusNameBySysName(ProjectStatusNameEnum.Moderation.ToString());
            var project = await _projectRepository.CreateProjectAsync(projectName, projectDetails, userId,
                ProjectStatusNameEnum.Moderation.ToString(), statusName, projectStage);

            // Если что то пошло не так при создании проекта.
            if (project?.ProjectId <= 0)
            {
                var ex = new Exception("Ошибка при создании проекта.");
                await _logService.LogErrorAsync(ex);
                await _projectNotificationsService.SendNotificationErrorCreatedUserProjectAsync("Что то пошло не так",
                    "Ошибка при создании проекта. Мы уже знаем о проблеме и уже занимаемся ей.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, userId);

                return null;
            }

            // Отправляем уведомление об успешном создании проекта.
            await _projectNotificationsService.SendNotificationSuccessCreatedUserProjectAsync("Все хорошо",
                "Данные успешно сохранены. Проект отправлен на модерацию.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, userId);

            return project;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает названия полей для таблицы проектов пользователя.
    /// Все названия столбцов этой таблицы одинаковые у всех пользователей.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    public async Task<IEnumerable<ProjectColumnNameOutput>> UserProjectsColumnsNamesAsync()
    {
        try
        {
            var items = await _projectRepository.UserProjectsColumnsNamesAsync();

            if (!items.Any())
            {
                throw new InvalidOperationException("Не удалось получить поля для таблицы ProjectColumnsNames.");
            }

            var result = _mapper.Map<IEnumerable<ProjectColumnNameOutput>>(items);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список проектов пользователя.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список проектов.</returns>
    public async Task<UserProjectResultOutput> UserProjectsAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                throw new NotFoundUserIdByAccountException(account);
            }

            var result = await _projectRepository.UserProjectsAsync(userId);

            foreach (var prj in result.UserProjects)
            {
                prj.ProjectDetails = ClearHtmlBuilder.Clear(prj.ProjectDetails);
            }

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// TODO: Подумать, давать ли всем пользователям возможность просматривать каталог проектов или только тем, у кого есть подписка.
    /// Метод получает список проектов для каталога.
    /// </summary>
    /// <returns>Список проектов.</returns>
    public async Task<CatalogProjectResultOutput> CatalogProjectsAsync()
    {
        try
        {
            var result = new CatalogProjectResultOutput
            {
                CatalogProjects = await _projectRepository.CatalogProjectsAsync()
            };
            
            if (!result.CatalogProjects.Any())
            {
                return result;
            }
            
            // Получаем список юзеров для проставления цветов.
            var userIds = result.CatalogProjects.Select(p => p.UserId).Distinct();
            
            // Выбираем список подписок пользователей.
            var userSubscriptions = await _subscriptionRepository.GetUsersSubscriptionsAsync(userIds);

            // Получаем список подписок.
            var subscriptions = await _subscriptionRepository.GetSubscriptionsAsync();
            
            // Получаем список тарифов, чтобы взять названия тарифов.
            var fareRules = await _fareRuleRepository.GetFareRulesAsync();
            var fareRulesList = fareRules.ToList();

            // Выбираем пользователей, у которых есть подписка выше бизнеса. Только их выделяем цветом.
            foreach (var prj in result.CatalogProjects)
            {
                // Смотрим подписку пользователя.
                var userSubscription = userSubscriptions.Find(s => s.UserId == prj.UserId);

                if (userSubscription is null)
                {
                    continue;
                }
                
                var subscriptionId = userSubscription.SubscriptionId;
                var s = subscriptions.Find(s => s.ObjectId == subscriptionId);

                if (s is null)
                {
                    continue;
                }
                
                // Получаем название тарифа подписки.
                var sn = fareRulesList.Find(fr => fr.RuleId == s.ObjectId);
                
                if (sn is null)
                {
                    continue;
                }
                        
                // Подписка позволяет. Проставляем выделение цвета.
                if (_fareRuleTypesNames.Contains(sn.Name))
                {
                    prj.IsSelectedColor = true;
                }
            }

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод обновляет проект пользователя.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectStage">Стадия проекта.</param>
    /// <returns>Данные нового проекта.</returns>
    public async Task<UpdateProjectOutput> UpdateProjectAsync(string projectName, string projectDetails, string account,
        long projectId, ProjectStageEnum projectStage)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                await _projectNotificationsService.SendNotificationErrorUpdatedUserProjectAsync("Что то не так...",
                    "Ошибка при обновлении проекта. Мы уже знаем о проблеме и уже занимаемся ей.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, userId);
                throw ex;
            }

            if (projectId <= 0)
            {
                await ValidateProjectIdAsync(projectId, userId);
            }

            // Изменяем проект в БД.
            var result =
                await _projectRepository.UpdateProjectAsync(projectName, projectDetails, userId, projectId,
                    projectStage);

            // TODO: Добавить отправку проекта на модерацию тут. Также удалять проект из каталога проектов на время модерации.
            await _projectNotificationsService.SendNotificationSuccessUpdatedUserProjectAsync("Все хорошо",
                "Данные успешно изменены. Проект отправлен на модерацию.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, userId);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает проект для изменения или просмотра.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="mode">Режим. Чтение или изменение.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные проекта.</returns>
    public async Task<ProjectOutput> GetProjectAsync(long projectId, ModeEnum mode, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var prj = await _projectRepository.GetProjectAsync(projectId);

            if (prj.Item1 is null)
            {
                var ex = new InvalidOperationException(
                    $"Не удалось найти проект с ProjectId {projectId} и UserId {userId}");
                throw ex;
            }

            var result = await CreateProjectResultAsync(projectId, prj, userId);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает стадии проекта для выбора.
    /// </summary>
    /// <returns>Стадии проекта.</returns>
    public async Task<IEnumerable<ProjectStageOutput>> ProjectStagesAsync()
    {
        try
        {
            var items = await _projectRepository.ProjectStagesAsync();
            var result = _mapper.Map<IEnumerable<ProjectStageOutput>>(items);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список вакансий проекта. Список вакансий, которые принадлежат владельцу проекта.
    /// </summary>
    /// <param name="projectId">Id проекта, вакансии которого нужно получить.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список вакансий.</returns>
    public async Task<ProjectVacancyResultOutput> ProjectVacanciesAsync(long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            if (projectId <= 0)
            {
                await ValidateProjectIdAsync(projectId, userId);
            }

            var result = new ProjectVacancyResultOutput
            {
                ProjectVacancies = new List<ProjectVacancyOutput>()
            };
            
            // Проставляем признаки видимости кнопок вакансий проекта.
            result = await FillVisibleControlsProjectVacanciesAsync(result, projectId, userId);
            
            var items = await _projectRepository.ProjectVacanciesAsync(projectId);

            if (items is null || !items.Any())
            {
                return result;
            }

            result.ProjectVacancies = _mapper.Map<IEnumerable<ProjectVacancyOutput>>(items);
            var projectVacancies = result.ProjectVacancies.ToList();

            // Проставляем вакансиям статусы.
            result.ProjectVacancies = await FillVacanciesStatuses(projectVacancies);

            // Чистим описания от html-тегов.
            result.ProjectVacancies = ClearHtmlTags(projectVacancies);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод создает вакансию проекта. При этом автоматически происходит привязка к проекту.
    /// </summary>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="workExperience">Опыт работы.</param>
    /// <param name="employment">Занятость у вакансии.</param>
    /// <param name="payment">Оплата у вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Данные вакансии.</returns>
    public async Task<UserVacancyEntity> CreateProjectVacancyAsync(string vacancyName, string vacancyText,
        long projectId, string employment, string payment, string workExperience, string account)
    {
        try
        {
            // Если невалидный Id проекта.
            if (projectId <= 0)
            {
                var ex = new ArgumentException("Невалидный Id проекта. ProjectId был " + projectId);
                await _logService.LogErrorAsync(ex);
                throw ex;
            }

            // Создаем вакансию.
            var createdVacancy = await _vacancyService.CreateVacancyAsync(vacancyName, vacancyText, workExperience,
                employment, payment, account);

            // Автоматически привязываем вакансию к проекту.
            await AttachProjectVacancyAsync(projectId, createdVacancy.VacancyId, account);

            return createdVacancy;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список вакансий проекта, которые могут быть прикреплены у проекту пользователя.
    /// </summary>
    /// <param name="projectId">Id проекта, для которого получить список вакансий.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список вакансий проекта.</returns>
    public async Task<IEnumerable<ProjectVacancyEntity>> ProjectVacanciesAvailableAttachAsync(long projectId,
        string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                await _logService.LogErrorAsync(ex);
                throw ex;
            }

            // Получаем список вакансий проекта, из которых можно выбрать вакансию для прикрепления к проекту.
            // Исключаем вакансии, которые уже прикреплены к проекту.
            var result = await _projectRepository.ProjectVacanciesAvailableAttachAsync(projectId, userId);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод прикрепляет вакансию к проекту.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    public async Task AttachProjectVacancyAsync(long projectId, long vacancyId, string account)
    {
        try
        {
            var isDublicate = await _projectRepository.AttachProjectVacancyAsync(projectId, vacancyId);
            
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                await _logService.LogErrorAsync(ex);
                throw ex;
            }

            if (isDublicate)
            {
                var ex = new DublicateProjectVacancyException();
                await _logService.LogErrorAsync(ex);
                await _projectNotificationsService.SendNotificationErrorDublicateAttachProjectVacancyAsync(
                    "Что то не так...",
                    GlobalConfigKeys.ProjectVacancy.DUBLICATE_PROJECT_VACANCY,
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, userId);
                throw ex;
            }

            await _projectNotificationsService.SendNotificationSuccessAttachProjectVacancyAsync(
                "Все хорошо",
                "Вакансия успешно привязана к проекту.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS,
                userId);
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод записывает отклик на проект.
    /// Отклик может быть с указанием вакансии, на которую идет отклик (если указана VacancyId).
    /// Отклик может быть без указаниея вакансии, на которую идет отклик (если не указана VacancyId).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Выходная модель с записанным откликом.</returns>
    public async Task<ProjectResponseEntity> WriteProjectResponseAsync(long projectId, long? vacancyId, string account)
    {
        long userId = 0;
        
        try
        {
            userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                await _logService.LogErrorAsync(ex);
                throw ex;
            }

            var result = await _projectRepository.WriteProjectResponseAsync(projectId, vacancyId, userId);

            // Показываем уведомления.
            await DisplayNotificationsAfterResponseProjectAsync(vacancyId, result.ResponseId, userId);

            var projectName = await _projectRepository.GetProjectNameByProjectIdAsync(projectId);

            // Записываем уведомления о приглашении в проект.
            await _projectNotificationsRepository.AddNotificationInviteProjectAsync(
                projectId, vacancyId, userId, projectName);
            
            return result;
        }

        catch (DublicateProjectResponseException ex)
        {
            await _projectNotificationsService.SendNotificationWarningProjectResponseAsync(
                "Внимание",
                "Вы уже откликались на этот проект.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, userId);
            await _logService.LogErrorAsync(ex);
            throw;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает команду проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные команды проекта.</returns>
    public async Task<IEnumerable<ProjectTeamOutput>> GetProjectTeamAsync(long projectId)
    {
        try
        {
            // Получаем данные команды проекта.
            var projectTeam = await _projectRepository.GetProjectTeamAsync(projectId);

            // Если команды проекта не нашли.
            if (projectTeam is null)
            {
                var ex = new InvalidOperationException($"Команды проекта не найдено. ProjectId = {projectId}");
                throw ex;
            }

            // Находим участников команды проекта.
            var teamMembers = await _projectRepository.GetProjectTeamMembersAsync(projectTeam.TeamId);

            // Если не нашли участников команды проекта.
            if (teamMembers is null)
            {
                var ex = new InvalidOperationException(
                    $"Участников команды проекта не найдено. ProjectId = {projectId}. TeamId = {projectTeam.TeamId}");
                throw ex;
            }

            var result = await CreateProjectTeamResultAsync(teamMembers);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает названия полей для таблицы команды проекта пользователя.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    public async Task<IEnumerable<ProjectTeamColumnNameEntity>> ProjectTeamColumnsNamesAsync()
    {
        try
        {
            var result = await _projectRepository.ProjectTeamColumnsNamesAsync();

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод добавляет в команду проекта пользователей.
    /// </summary>
    /// <param name="inviteText">Текст, который будет использоваться для поиска пользователя для приглашения.</param>
    /// <param name="inviteType">Способ приглашения.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Добавленный пользователь.</returns>
    public async Task<ProjectTeamMemberEntity> InviteProjectTeamAsync(string inviteText,
        ProjectInviteTypeEnum inviteType, long projectId, long? vacancyId, string account)
    {
        try
        {
            await ValidateInviteProjectTeamParams(inviteText, inviteType, projectId, vacancyId, account);

            var userId = await GetUserIdAsync(inviteText, inviteType);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(inviteText);
                await _logService.LogErrorAsync(ex);
                throw ex;
            }

            // Получаем Id команды проекта.
            var teamId = await _projectRepository.GetProjectTeamIdAsync(projectId);

            // Добавляем пользователя в команду проекта.
            var result = await _projectRepository.AddProjectTeamMemberAsync(userId, vacancyId, teamId);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex,
                "Ошибка добавления пользователя в команду проекта. " +
                $"InviteText был {inviteText}. " +
                $"InviteType был {inviteType}. " +
                $"ProjectId был {projectId}. " +
                $"VacancyId был {vacancyId}");
            throw;
        }
    }

    /// <summary>
    /// Метод фильтрации проектов в зависимости от параметров фильтров.
    /// </summary>
    /// <param name="filterProjectInput">Входная модель.</param>
    /// <returns>Список проектов после фильтрации.</returns>
    public async Task<IEnumerable<CatalogProjectOutput>> FilterProjectsAsync(FilterProjectInput filters)
    {
        try
        {
            // Разбиваем строку стадий проекта, так как там может приходить несколько значений в строке.
            filters.ProjectStages = CreateProjectStagesBuilder.CreateProjectStagesResult(filters.StageValues);
            var items = await _projectRepository.GetFiltersProjectsAsync();
            var result = await _dateProjectsFilterChain.FilterProjectsAsync(filters, items);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод удаляет вакансию проекта.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    public async Task DeleteProjectVacancyAsync(long vacancyId, long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
        
            // Только владелец проекта может удалять вакансии проекта.
            var isOwner = await _projectRepository.CheckProjectOwnerAsync(projectId, userId);

            if (!isOwner)
            {
                var ex = new InvalidOperationException(
                    "Пользователь не является владельцем проекта. " +
                    $"UserId: {userId}. " +
                    $"ProjectId: {projectId}. " +
                    $"VacancyId: {vacancyId}");
                throw ex;
            }

            var isRemoved = await _projectRepository.DeleteProjectVacancyByIdAsync(vacancyId, projectId);

            if (!isRemoved)
            {
                var ex = new InvalidOperationException(
                    "Ошибка удаления вакансии проекта. " +
                    $"VacancyId: {vacancyId}. " +
                    $"ProjectId: {projectId}. " +
                    $"UserId: {userId}");
            
                await _projectNotificationsService.SendNotificationErrorDeleteProjectVacancyAsync(
                    "Ошибка",
                    "Ошибка при удалении вакансии проекта.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, userId);
                throw ex;
            }
        
            await _projectNotificationsService.SendNotificationSuccessDeleteProjectVacancyAsync(
                "Все хорошо",
                "Вакансия успешно удалена из проекта.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, userId);
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод удаляет проект и все, что с ним связано.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    public async Task DeleteProjectAsync(long projectId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
        
            // Только владелец проекта может удалять проект.
            var isOwner = await _projectRepository.CheckProjectOwnerAsync(projectId, userId);

            if (!isOwner)
            {
                var ex = new InvalidOperationException(
                    "Пользователь не является владельцем проекта. " +
                    $"UserId: {userId}. " +
                    $"ProjectId: {projectId}.");
                throw ex;
            }

            var isRemoved = await _projectRepository.DeleteProjectAsync(projectId, userId);
            
            if (!isRemoved)
            {
                var ex = new InvalidOperationException(
                    "Ошибка удаления проекта. " +
                    $"ProjectId: {projectId}. " +
                    $"UserId: {userId}");
            
                await _projectNotificationsService.SendNotificationErrorDeleteProjectAsync(
                    "Ошибка",
                    "Ошибка при удалении проекта.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, userId);
                throw ex;
            }
        
            await _projectNotificationsService.SendNotificationSuccessDeleteProjectAsync(
                "Все хорошо",
                "Проект успешно удален.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, userId);
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
    
    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод записывает данные участников команды проекта.
    /// </summary>
    /// <param name="teamMembers">Список участников команды проекта.</param>
    /// <returns>Список с изменениями.</returns>
    private async Task<List<ProjectTeamOutput>> CreateProjectTeamResultAsync(
        IEnumerable<ProjectTeamMemberEntity> teamMembers)
    {
        var result = new List<ProjectTeamOutput>();
        
        foreach (var member in teamMembers)
        {
            // Заполняем название вакансии.
            var vacancyName = await _vacancyRepository.GetVacancyNameByVacancyIdAsync(member.UserVacancy.VacancyId);

            if (string.IsNullOrEmpty(vacancyName))
            {
                var ex = new InvalidOperationException(
                    $"Ошибка получения названия вакансии. VacancyId = {member.UserVacancy.VacancyId}");
                await _logService.LogErrorAsync(ex);
                throw ex;
            }
            
            var user = await _userRepository.GetUserByUserIdAsync(member.UserId);

            if (user is null)
            {
                var ex = new InvalidOperationException(
                    $"Ошибка получения данных пользователя. UserId = {member.UserId}");
                throw ex;
            }

            // Создаем команду проекта.
            var team = CreateProjectTeamResult(vacancyName, user, member);
            
            result.Add(team);
        }

        return result;
    }
    
    /// <summary>
    /// Метод валидирует входные параметры перед добавлением пользователя в команду проекта.
    /// </summary>
    /// <param name="inviteText">Текст, который будет использоваться для поиска пользователя для приглашения.</param>
    /// <param name="inviteType">Способ приглашения.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    private async Task ValidateInviteProjectTeamParams(string inviteText, ProjectInviteTypeEnum inviteType,
        long projectId, long? vacancyId, string account)
    {
        var isError = false;

        if (string.IsNullOrEmpty(inviteText))
        {
            var ex = new ArgumentException(ValidationConsts.NOT_VALID_INVITE_PROJECT_TEAM_USER);
            await _logService.LogErrorAsync(ex);
            isError = true;
        }
        
        if (!_projectInviteTypes.Contains(inviteType))
        {
            var ex = new ArgumentException(ValidationConsts.NOT_VALID_INVITE_TYPE);
            await _logService.LogErrorAsync(ex);
            isError = true;
        }

        if (projectId <= 0)
        {
            var ex = new ArgumentException(ValidationConsts.NOT_VALID_INVITE_PROJECT_TEAM_PROJECT_ID);
            await _logService.LogErrorAsync(ex);
            isError = true;
        }

        if (vacancyId <= 0)
        {
            var ex = new ArgumentException(ValidationConsts.NOT_VALID_INVITE_PROJECT_TEAM_VACANCY_ID);
            await _logService.LogErrorAsync(ex);
            isError = true;
        }
        
        var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        // Если была ошибка, то покажем уведомление юзеру и генерим исключение.
        if (isError)
        {
            await _projectNotificationsService.SendNotificationErrorInviteProjectTeamMembersAsync("Ошибка",
                "Ошибка при добавлении пользователя в команду проекта. Мы уже знаем о ней и разбираемся. " +
                "А пока, попробуйте еще раз.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, userId);
        }
    }
    
    /// <summary>
    /// Метод валидирует Id проекта. Выбрасываем исклчюение, если он невалидный.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    private async Task ValidateProjectIdAsync(long projectId, long userId)
    {
        var ex = new ArgumentNullException(string.Concat(ValidationConsts.NOT_VALID_PROJECT_ID, projectId));
        await _logService.LogErrorAsync(ex);
        await _projectNotificationsService.SendNotificationErrorUpdatedUserProjectAsync("Что то не так...",
            "Ошибка при обновлении проекта. Мы уже знаем о проблеме и уже занимаемся ей.",
            NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, userId);
        throw ex;
    }

    /// <summary>
    /// Метод проставляет статусы вакансиям.
    /// </summary>
    /// <param name="projectVacancies">Список вакансий.</param>
    /// <returns>Список вакансий.</returns>
    private async Task<IEnumerable<ProjectVacancyOutput>> FillVacanciesStatuses(
        List<ProjectVacancyOutput> projectVacancies)
    {
        // Получаем список вакансий на модерации.
        var moderationVacancies = await _vacancyModerationService.VacanciesModerationAsync();

        // Получаем список вакансий из каталога вакансий.
        var catalogVacancies = await _vacancyRepository.CatalogVacanciesAsync();

        // Проставляем статусы вакансий.
        foreach (var pv in projectVacancies)
        {
            // Ищем в модерации вакансий.
            var isVacancy = moderationVacancies.Vacancies.Any(v => v.VacancyId == pv.VacancyId);

            if (isVacancy)
            {
                pv.UserVacancy.VacancyStatusName = moderationVacancies.Vacancies
                    .Where(v => v.VacancyId == pv.VacancyId)
                    .Select(v => v.ModerationStatusName)
                    .FirstOrDefault();
            }
                
            // Ищем вакансию в каталоге вакансий.
            else
            {
                var isCatalogVacancy = catalogVacancies.Any(v => v.VacancyId == pv.VacancyId);

                if (isCatalogVacancy)
                {
                    pv.UserVacancy.VacancyStatusName = _approveVacancy;
                }
            }
        }

        return projectVacancies;
    }

    /// <summary>
    /// Метод чистит описание от тегов.
    /// </summary>
    /// <param name="projectVacancies">Список вакансий.</param>
    /// <returns>Список вакансий после очистки.</returns>
    private IEnumerable<ProjectVacancyOutput> ClearHtmlTags(List<ProjectVacancyOutput> projectVacancies)
    {
        // Чистим описание вакансии от html-тегов.
        foreach (var vac in projectVacancies)
        {
            vac.UserVacancy.VacancyText = ClearHtmlBuilder.Clear(vac.UserVacancy.VacancyText);
        }

        return projectVacancies;
    }
    
    /// <summary>
    /// Метод создает результаты проекта. 
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="prj">Данные проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Результаты проекта.</returns>
    private async Task<ProjectOutput> CreateProjectResultAsync(long projectId,
        (UserProjectEntity, ProjectStageEntity) prj, long userId)
    {
        var result = _mapper.Map<ProjectOutput>(prj.Item1);
        result.StageId = prj.Item2.StageId;
        result.StageName = prj.Item2.StageName;
        result.StageSysName = prj.Item2.StageSysName;
        
        // Проверяем владельца проекта.
        var projectOwnerId = await _projectRepository.GetProjectOwnerIdAsync(projectId);

        // Если владелец проекта, то проставляем признак видимости кнопок событий.
        if (projectOwnerId == userId)
        {
            result.IsVisibleDeleteButton = true;
        }

        else
        {
            // Просматривает не владелец, допускаем видимость кнопок действий проекта.
            result.IsVisibleActionProjectButtons = true;
        }

        return result;
    }

    /// <summary>
    /// Метод проставляет признаки видимости кнопок вакансий проекта.
    /// </summary>
    /// <param name="result">Результирующая модель.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Результирующая модель.</returns>
    private async Task<ProjectVacancyResultOutput> FillVisibleControlsProjectVacanciesAsync(
        ProjectVacancyResultOutput result, long projectId, long userId)
    {
        // Проверяем владельца проекта.
        var projectOwnerId = await _projectRepository.GetProjectOwnerIdAsync(projectId);

        // Если владелец проекта, то проставляем признак видимости кнопок событий.
        if (projectOwnerId == userId)
        {
            result.IsVisibleActionVacancyButton = true;
        }

        return result;
    }

    /// <summary>
    /// Метод записывает участника команды проекта.
    /// </summary>
    /// <param name="user">Данные пользователя.</param>
    /// <returns>Участник.</returns>
    private string FillProjectTeamMemberAsync(UserEntity user)
    {
        // Если у пользователя заполнены имя и фамилия, то запишем их.
        if (!string.IsNullOrEmpty(user.FirstName)
            && !string.IsNullOrEmpty(user.LastName))
        {
            return user.FirstName + " " + user.LastName;
        }

        // Если логин заполнен, то запишем его.
        if (!string.IsNullOrEmpty(user.Login))
        {
            return user.Login;
        }

        // Иначе запишем Email.
        return user.Email;
    }

    /// <summary>
    /// Метод форматирует дату к нужному виду.
    /// </summary>
    /// <param name="date">Дата, которую нужно форматировать.</param>
    /// <returns>Дата в нужном виде.</returns>
    private string CreateDateResult(DateTime date)
    {
        return date.ToString("g", CultureInfo.GetCultureInfo("ru"));
    }

    /// <summary>
    /// Метод создает результат команды проекта.
    /// </summary>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="user">Данные пользователя.</param>
    /// <param name="member">Данные участника.</param>
    /// <returns>Результирующая модель.</returns>
    private ProjectTeamOutput CreateProjectTeamResult(string vacancyName, UserEntity user,
        ProjectTeamMemberEntity member)
    {
        var team = new ProjectTeamOutput
        {
            VacancyName = vacancyName,
            Member = FillProjectTeamMemberAsync(user), // Заполняем участника команды проекта.
            Joined = CreateDateResult(member.Joined), // Форматируем даты.
            UserId = member.UserId
        };

        return team;
    }

    /// <summary>
    /// Метод отправляет уведомления после отклика на проект.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    private async Task DisplayNotificationsAfterResponseProjectAsync(long? vacancyId, long responseId, long userId)
    {
        if (responseId > 0)
        {
            await _projectNotificationsService.SendNotificationSuccessProjectResponseAsync(
                "Все хорошо",
                "Отклик на проект успешно оставлен. Вы получите уведомление о решении владельца проекта.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, userId);
        }

        else
        {
            if (vacancyId > 0)
            {
                await _projectNotificationsService.SendNotificationErrorProjectResponseAsync("Ошибка",
                    "Ошибка при отклике на проект с указанием вакансии. Мы уже знаем о ней и разбираемся. " +
                    "А пока, попробуйте еще раз.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, userId);
            }

            else
            {
                await _projectNotificationsService.SendNotificationErrorProjectResponseAsync("Ошибка",
                    "Ошибка при отклике на проект без указания вакансии. Мы уже знаем о ней и разбираемся. " +
                    "А пока, попробуйте еще раз.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, userId);
            }
        }
    }

    /// <summary>
    /// Метод находит Id пользователя выбранным способом.
    /// </summary>
    /// <param name="inviteText">Текст, который будет использоваться для поиска пользователя для приглашения.</param>
    /// <param name="inviteType">Способ приглашения.</param>
    /// <returns>Id пользователя.</returns>
    private async Task<long> GetUserIdAsync(string inviteText, ProjectInviteTypeEnum inviteType)
    {
        var projectInviteTeamJob = new ProjectInviteTeamJob();

        var userId = inviteType switch
        {
            ProjectInviteTypeEnum.Link => await projectInviteTeamJob.GetUserIdAsync(
                new ProjectInviteTeamLinkStrategy(_userRepository, _projectNotificationsService), inviteText),

            ProjectInviteTypeEnum.Email => await projectInviteTeamJob.GetUserIdAsync(
                new ProjectInviteTeamEmailStrategy(_userRepository, _projectNotificationsService), inviteText),

            ProjectInviteTypeEnum.PhoneNumber => await projectInviteTeamJob.GetUserIdAsync(
                new ProjectInviteTeamPhoneNumberStrategy(_userRepository, _projectNotificationsService), inviteText),

            ProjectInviteTypeEnum.Login => await projectInviteTeamJob.GetUserIdAsync(
                new ProjectInviteTeamLoginStrategy(_userRepository, _projectNotificationsService), inviteText),

            _ => 0
        };

        return userId;
    }
    
    #endregion
}