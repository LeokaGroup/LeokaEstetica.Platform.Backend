using System.Globalization;
using System.Runtime.CompilerServices;
using AutoMapper;
using LeokaEstetica.Platform.Access.Abstractions.AvailableLimits;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Access.Consts;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Database.Abstractions.Notification;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Finder.Chains.Project;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectTeam;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.ProjectTeam;
using LeokaEstetica.Platform.Models.Entities.User;
using LeokaEstetica.Platform.CallCenter.Abstractions.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using LeokaEstetica.Platform.Services.Abstractions.Vacancy;
using LeokaEstetica.Platform.Services.Builders;
using LeokaEstetica.Platform.Services.Consts;
using LeokaEstetica.Platform.Services.Strategies.Project.Team;
using LeokaEstetica.Platform.Base.Extensions.HtmlExtensions;
using LeokaEstetica.Platform.Core.Extensions;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Services.Helpers;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.Project;

/// <summary>
/// Класс реализует методы сервиса проектов.
/// </summary>
internal sealed class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<ProjectService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IProjectNotificationsService _projectNotificationsService;
    private readonly IVacancyService _vacancyService;
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IFillColorProjectsService _fillColorProjectsService;

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
    private readonly IMailingsService _mailingsService;

   
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
    private readonly IAccessUserNotificationsService _accessUserNotificationsService;
    private readonly IAccessUserService _accessUserService;
    private readonly IProjectModerationRepository _projectModerationRepository;
    
    private const string NOT_AVAILABLE_DELETE_PROJECT_ARCHIVE = "Невозможно убрать проект из архива, так как у Вас уже опубликовано максимальное количество проектов соответствующих максимальному лимиту тарифа. Добавьте в архив проекты, чтобы освободить лимиты либо перейдите на тариф, который имеет большие лимиты";

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    /// <param name="_logger">Сервис логера.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="projectNotificationsService">Сервис уведомлений.</param>
    /// <param name="vacancyService">Сервис вакансий.</param>
    /// <param name="vacancyRepository">Репозиторий вакансий.</param>
    /// <param name="availableLimitsService">Сервис проверки лимитов.</param>
    /// <param name="vacancyModerationService">Сервис модерации вакансий проектов.</param>
    /// <param name="notificationsRepository">Репозиторий уведомлений.</param>
    /// <param name="accessUserNotificationsService">Сервис уведомлений доступа пользователя.</param>
    /// <param name="accessUserService">Сервис доступа пользователя.</param>
    /// <param name="projectModerationRepository">Репозиторий модерации проектов.</param>
    public ProjectService(IProjectRepository projectRepository,
        ILogger<ProjectService> logger,
        IUserRepository userRepository,
        IMapper mapper,
        IProjectNotificationsService projectNotificationsService,
        IVacancyService vacancyService,
        IVacancyRepository vacancyRepository, 
        IAvailableLimitsService availableLimitsService, 
        ISubscriptionRepository subscriptionRepository, 
        IFareRuleRepository fareRuleRepository, 
        IVacancyModerationService vacancyModerationService, 
        IProjectNotificationsRepository projectNotificationsRepository, 
        IAccessUserNotificationsService accessUserNotificationsService, 
        IAccessUserService accessUserService,
        IFillColorProjectsService fillColorProjectsService, 
        IMailingsService mailingsService, 
        IProjectModerationRepository projectModerationRepository)
    {
        _projectRepository = projectRepository;
        _logger = logger;
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
        _accessUserNotificationsService = accessUserNotificationsService;
        _accessUserService = accessUserService;
        _fillColorProjectsService = fillColorProjectsService;
        _mailingsService = mailingsService;
        _projectModerationRepository = projectModerationRepository;

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
    /// <param name="createProjectInput">Входная модель.</param>
    /// <returns>Данные нового проекта.</returns>
    public async Task<UserProjectEntity> CreateProjectAsync(CreateProjectInput createProjectInput)
    {
        try
        {
            var account = createProjectInput.Account;
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            // Проверяем заполнение анкеты и даем доступ либо нет.
            var isEmptyProfile = await _accessUserService.IsProfileEmptyAsync(userId);

            var token = createProjectInput.Token;

            // Если нет доступа, то не даем оплатить платный тариф.
            if (isEmptyProfile)
            {
                var ex = new InvalidOperationException($"Анкета пользователя не заполнена. UserId был: {userId}");

                await _accessUserNotificationsService.SendNotificationWarningEmptyUserProfileAsync("Внимание",
                    "Для создания проекта должна быть заполнена информация вашей анкеты.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);
                
                throw ex;
            }
            
            // Получаем подписку пользователя.
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(userId);
            
            // Получаем тариф, на который оформлена подписка у пользователя.
            var fareRule = await _fareRuleRepository.GetByIdAsync(userSubscription.ObjectId);
            
            // Проверяем доступно ли пользователю создание проекта.
            var availableCreateProjectLimit = await _availableLimitsService.CheckAvailableCreateProjectAsync(userId,
                fareRule.Name);

            // Если лимит по тарифу превышен.
            if (!availableCreateProjectLimit)
            {
                var ex = new InvalidOperationException("Превышен лимит проектов по тарифу. " +
                                                       $"UserId: {userId}. " +
                                                       $"Тариф: {fareRule.Name}");

                await _projectNotificationsService.SendNotificationWarningLimitFareRuleProjectsAsync(
                    "Что то пошло не так",
                    "Превышен лимит проектов по тарифу.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);

                throw ex;
            }

            var projectName = createProjectInput.ProjectName;

            // Проверяем существование такого проекта у текущего пользователя.
            var isCreatedProject = await _projectRepository.CheckCreatedProjectByProjectNameAsync(projectName, userId);

            // Есть дубликат, нельзя создать проект.
            if (isCreatedProject)
            {
                var ex = new InvalidOperationException($"Попытка создать дубликат проекта. UserId: {userId}");
                
                await _projectNotificationsService
                    .SendNotificationWarningDublicateUserProjectAsync("Увы...", "Такой проект у вас уже существует.",
                        NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);

                throw ex;
            }

            createProjectInput.UserId = userId;

            // Создаем проект.
            var project = await _projectRepository.CreateProjectAsync(createProjectInput);
            var projectId = project.ProjectId;

            // Если что то пошло не так при создании проекта.
            if (project is null || projectId <= 0)
            {
                var ex = new InvalidOperationException("Ошибка при создании проекта.");
                _logger.LogError(ex, ex.Message);
                
                await _projectNotificationsService.SendNotificationErrorCreatedUserProjectAsync("Что то пошло не так",
                    "Ошибка при создании проекта. Мы уже знаем о проблеме и уже занимаемся ей.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);

                throw ex;
            }
            
            // Добавляем владельца в участники проекта по дефолту.
            await AddProjectOwnerToTeamMembersAsync(userId, projectId);

            // Отправляем уведомление об успешном создании проекта.
            await _projectNotificationsService.SendNotificationSuccessCreatedUserProjectAsync("Все хорошо",
                "Данные успешно сохранены. Проект отправлен на модерацию.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);

            var user = await _userRepository.GetUserPhoneEmailByUserIdAsync(userId);
            
            // Отправляем уведомление о созданном проекте владельцу проекта.
            await _mailingsService.SendNotificationCreatedProjectAsync(user.Email, projectName, projectId);

            return project;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список проектов пользователя.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="isCreateVacancy">Признак создания вакансии.</param>
    /// <returns>Список проектов.</returns>
    public async Task<UserProjectResultOutput> UserProjectsAsync(string account, bool isCreateVacancy)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                throw new NotFoundUserIdByAccountException(account);
            }

            var result = await _projectRepository.UserProjectsAsync(userId, isCreateVacancy);

            foreach (var prj in result.UserProjects)
            {
                prj.ProjectDetails = ClearHtmlBuilder.Clear(prj.ProjectDetails);
            }

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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
            var result = new CatalogProjectResultOutput { CatalogProjects = new List<CatalogProjectOutput>() };
            
            // Получаем список проектов для каталога.
            var projects = await _projectRepository.CatalogProjectsAsync();

            result.CatalogProjects = await ExecuteCatalogConditionsAsync(projects);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод обновляет проект пользователя.
    /// </summary>
    /// <param name="updateProjectInput">Входная модель.</param>
    /// <returns>Данные нового проекта.</returns>
    public async Task<UpdateProjectOutput> UpdateProjectAsync(UpdateProjectInput updateProjectInput)
    {
        try
        {
            var account = updateProjectInput.Account;
            var token = updateProjectInput.Token;
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                await _projectNotificationsService.SendNotificationErrorUpdatedUserProjectAsync("Что то не так...",
                    "Ошибка при обновлении проекта. Мы уже знаем о проблеме и уже занимаемся ей.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
                throw ex;
            }

            var projectId = updateProjectInput.ProjectId;

            if (projectId <= 0)
            {
                await ValidateProjectIdAsync(projectId, token);
            }
            
            updateProjectInput.UserId = userId;

            // Изменяем проект в БД.
            var result = await _projectRepository.UpdateProjectAsync(updateProjectInput);
            
            await _projectNotificationsService.SendNotificationSuccessUpdatedUserProjectAsync("Все хорошо",
                "Данные успешно изменены. Проект отправлен на модерацию.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
            
            // Проверяем наличие неисправленных замечаний.
            await CheckAwaitingCorrectionRemarksAsync(projectId);

            result.ProjectRemarks ??= new List<ProjectRemarkOutput>();

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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

            if (projectId <= 0)
            {
                throw new InvalidOperationException($"Не передан Id проекта. ProjectId: {projectId}");
            }
            
            // Проверяем доступ к проекту.
            var isOwner = await _projectRepository.CheckProjectOwnerAsync(projectId, userId);
            
            // Нет доступа на изменение.
            if (!isOwner && mode == ModeEnum.Edit)
            {
                return new ProjectOutput
                {
                    IsAccess = false,
                    IsSuccess = false,
                    ProjectRemarks = new List<ProjectRemarkOutput>()
                };
            }

            var prj = await _projectRepository.GetProjectAsync(projectId);

            if (prj.UserProject is null)
            {
                var ex = new InvalidOperationException( 
                    $"Не удалось найти проект с ProjectId {projectId} и UserId {userId}");
                throw ex;
            }

            var result = await CreateProjectResultAsync(projectId, prj, userId);

            var remarks = await _projectModerationRepository.GetProjectRemarksAsync(projectId);
            result.ProjectRemarks = _mapper.Map<IEnumerable<ProjectRemarkOutput>>(remarks);
            result.IsAccess = true;
            
            result.ProjectDetails = ClearHtmlBuilder.Clear(result.ProjectDetails);
            result.Conditions = ClearHtmlBuilder.Clear(result.Conditions);
            result.Demands = ClearHtmlBuilder.Clear(result.Demands);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список вакансий проекта. Список вакансий, которые принадлежат владельцу проекта.
    /// </summary>
    /// <param name="projectId">Id проекта, вакансии которого нужно получить.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Список вакансий.</returns>
    public async Task<ProjectVacancyResultOutput> ProjectVacanciesAsync(long projectId, string account, string token)
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
                await ValidateProjectIdAsync(projectId, token);
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
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод создает вакансию проекта. При этом автоматически происходит привязка к проекту.
    /// </summary>
    /// <param name="createProjectVacancyInput">Входная модель.</param>
    /// <returns>Данные вакансии.</returns>
    public async Task<VacancyOutput> CreateProjectVacancyAsync(CreateProjectVacancyInput createProjectVacancyInput)
    {
        try
        {
            // Если невалидный Id проекта.
            if (createProjectVacancyInput.ProjectId <= 0)
            {
                var ex = new ArgumentException("Невалидный Id проекта. ProjectId был " +
                                               createProjectVacancyInput.ProjectId);
                throw ex;
            }

            // Создаем вакансию.
            var createdVacancy = await _vacancyService.CreateVacancyAsync(new VacancyInput
            {
                VacancyName = createProjectVacancyInput.VacancyName,
                VacancyText = createProjectVacancyInput.VacancyText,
                WorkExperience = createProjectVacancyInput.WorkExperience,
                Employment = createProjectVacancyInput.Employment,
                Payment = createProjectVacancyInput.Payment,
                Account = createProjectVacancyInput.Account,
                ProjectId = createProjectVacancyInput.ProjectId
            });

            // Автоматически привязываем вакансию к проекту.
            await AttachProjectVacancyAsync(createProjectVacancyInput.ProjectId, createdVacancy.VacancyId,
                createProjectVacancyInput.Account, createProjectVacancyInput.Token);

            return createdVacancy;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список вакансий проекта, которые могут быть прикреплены у проекту пользователя.
    /// </summary>
    /// <param name="projectId">Id проекта, для которого получить список вакансий.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="isInviteProject">Признак приглашения в проект.</param>
    /// <returns>Список вакансий проекта.</returns>
    public async Task<IEnumerable<ProjectVacancyEntity>> ProjectVacanciesAvailableAttachAsync(long projectId,
        string account, bool isInviteProject)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            // Получаем список вакансий проекта, из которых можно выбрать вакансию для прикрепления к проекту.
            var result = await _projectRepository.ProjectVacanciesAvailableAttachAsync(projectId, userId,
                isInviteProject);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод прикрепляет вакансию к проекту.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task AttachProjectVacancyAsync(long projectId, long vacancyId, string account, string token)
    {
        try
        {
            var isDublicate = await _projectRepository.AttachProjectVacancyAsync(projectId, vacancyId);
            
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            if (isDublicate)
            {
                var ex = new DublicateProjectVacancyException();
                await _projectNotificationsService.SendNotificationErrorDublicateAttachProjectVacancyAsync(
                    "Что то не так...",
                    ValidationConst.ProjectVacancy.DUBLICATE_PROJECT_VACANCY,
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
                throw ex;
            }

            await _projectNotificationsService.SendNotificationSuccessAttachProjectVacancyAsync(
                "Все хорошо",
                "Вакансия успешно привязана к проекту.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS,
                token);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Выходная модель с записанным откликом.</returns>
    public async Task<ProjectResponseEntity> WriteProjectResponseAsync(long projectId, long? vacancyId, string account,
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
            
            // Проверяем заполнение анкеты и даем доступ либо нет.
            var isEmptyProfile = await _accessUserService.IsProfileEmptyAsync(userId);

            // Если нет доступа, то не даем оплатить платный тариф.
            if (isEmptyProfile)
            {
                var ex = new InvalidOperationException($"Анкета пользователя не заполнена. UserId был: {userId}");

                await _accessUserNotificationsService.SendNotificationWarningEmptyUserProfileAsync("Внимание",
                    "Для отклика на проект должна быть заполнена информация вашей анкеты.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);
                
                throw ex;
            }

            var result = await _projectRepository.WriteProjectResponseAsync(projectId, vacancyId, userId);

            // Показываем уведомления.
            await DisplayNotificationsAfterResponseProjectAsync(vacancyId, result.ResponseId, token);

            var projectName = await _projectRepository.GetProjectNameByProjectIdAsync(projectId);

            // Записываем уведомления о приглашении в проект.
            await _projectNotificationsRepository.AddNotificationInviteProjectAsync(
                projectId, vacancyId, userId, projectName);
            
            // Находим данные о пользователе, который оставляет отклик на проект.
            var otherUser = await _userRepository.GetUserPhoneEmailByUserIdAsync(userId);
            
            // Находим почту владельца проекта для отправки ему уведомления.
            var projectOwnerEmail = await _projectRepository.GetProjectOwnerEmailByProjectIdAsync(projectId);

            var vacancyName = string.Empty;

            if (vacancyId > 0)
            {
                // Находим название вакансии.
                vacancyName = await _projectRepository.GetProjectVacancyNameByIdAsync((long)vacancyId);   
            }

            // Отправляем уведомление на почту владельцу проекта.
            await _mailingsService.SendNotificationWriteResponseProjectAsync(projectOwnerEmail, projectId, projectName,
                vacancyName, otherUser.Email);
            
            return result;
        }

        catch (DublicateProjectResponseException ex)
        {
            await _projectNotificationsService.SendNotificationWarningProjectResponseAsync(
                "Внимание",
                "Вы уже откликались на этот проект.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);
            
            _logger.LogError(ex, ex.Message);
            throw;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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
            _logger.LogError(ex, ex.Message);
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
            _logger.LogError(ex, ex.Message);
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
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Добавленный пользователь.</returns>
    public async Task<ProjectTeamMemberEntity> InviteProjectTeamAsync(string inviteText,
        ProjectInviteTypeEnum inviteType, long projectId, long? vacancyId, string account, string token)
    {
        try
        {
            await ValidateInviteProjectTeamParams(inviteText, inviteType, projectId, vacancyId, account, token);
            
            var currentUserId = await _userRepository.GetUserIdByEmailAsync(account);
            
            if (currentUserId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(inviteText);
                throw ex;
            }
            
            // Проверяем заполнение анкеты и даем доступ либо нет.
            var isEmptyProfile = await _accessUserService.IsProfileEmptyAsync(currentUserId);

            // Если нет доступа, то не даем оплатить платный тариф.
            if (isEmptyProfile)
            {
                var ex = new InvalidOperationException(
                    $"Анкета пользователя не заполнена. UserId был: {currentUserId}");

                await _accessUserNotificationsService.SendNotificationWarningEmptyUserProfileAsync("Внимание",
                    "Для приглашения пользователей в проект должна быть заполнена информация вашей анкеты.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);
                
                throw ex;
            }

            // Находим Id пользователя, которого приглашаем в проект.
            var inviteUserId = await GetUserIdAsync(inviteText, inviteType);

            // Проверяем нахождение проекта на модерации.
            var isProjectModeration = await _projectRepository.CheckProjectModerationAsync(projectId);

            // Если он там есть, то не даем пригласить в него.
            if (isProjectModeration)
            {
                var ex = new InvalidOperationException(
                    "Проект еще на модерации. Нельзя пригласить пользователей, пока проект не пройдет модерацию.");
                
                await _projectNotificationsService.SendNotificationWarningProjectInviteTeamAsync(
                    "Внимание",
                    "Проект еще на модерации. Нельзя пригласить пользователей, пока проект не пройдет модерацию.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);
                
                throw ex;
            }

            // Получаем Id команды проекта.
            var teamId = await _projectRepository.GetProjectTeamIdAsync(projectId);
            
            var isInvitedUser = await _projectRepository.CheckProjectTeamMemberAsync(teamId, inviteUserId);

            // Проверяем, не приглашали ли уже пользователя в команду проекта. Если да, то не даем пригласить повторно.
            if (isInvitedUser)
            {
                var ex = new InvalidOperationException("Пользователь уже был приглашен в команду проекта. " +
                                                       $"TeamId: {teamId}. " +
                                                       $"InvitedUserId: {inviteUserId}. " +
                                                       $"CurrentUserId: {currentUserId}");
                
                await _projectNotificationsService.SendNotificationWarningUserAlreadyProjectInvitedTeamAsync(
                    "Внимание",
                    "Пользователь уже был добавлен в команду проекта.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);
                
                throw ex;
            }

            // Добавляем пользователя в команду проекта.
            var result = await _projectRepository.AddProjectTeamMemberAsync(inviteUserId, vacancyId, teamId);
            
            // Находим название проекта.
            var projectName = await _projectRepository.GetProjectNameByProjectIdAsync(projectId);

            await SendEmailNotificationInviteTeamProjectAsync(projectId, inviteUserId, projectName);
            
            // Записываем уведомления о приглашении в проект.
            await _projectNotificationsRepository.AddNotificationInviteProjectAsync(projectId, vacancyId, inviteUserId,
                projectName);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка добавления пользователя в команду проекта. " +
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
           
            // Получаем список проектов для каталога.
            var projects = await _projectRepository.CatalogProjectsWithoutMemoryAsync();
            
            var result = await _dateProjectsFilterChain.FilterProjectsAsync(filters, projects);

            var resultProjects = await ExecuteCatalogConditionsAsync(result);

            return resultProjects;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод удаляет вакансию проекта.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    public async Task DeleteProjectVacancyAsync(long vacancyId, long projectId, string account, string token)
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
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
                throw ex;
            }
        
            await _projectNotificationsService.SendNotificationSuccessDeleteProjectVacancyAsync(
                "Все хорошо",
                "Вакансия успешно удалена из проекта.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод удаляет проект и все, что с ним связано.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    public async Task DeleteProjectAsync(long projectId, string account, string token)
    {
        try
        {
            if (projectId <= 0)
            {
                var ex = new ArgumentNullException($"Id проекта не может быть пустым. ProjectId: {projectId}");
                throw ex;
            }
            
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

            var removedProject = await _projectRepository.DeleteProjectAsync(projectId, userId);
            
            if (!removedProject.Success)
            {
                var ex = new InvalidOperationException(
                    "Ошибка удаления проекта. " +
                    $"ProjectId: {projectId}. " +
                    $"UserId: {userId}");

                if (!string.IsNullOrEmpty(token))
                {
                    await _projectNotificationsService.SendNotificationErrorDeleteProjectAsync(
                        "Ошибка",
                        "Ошибка при удалении проекта.",
                        NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);    
                }
                
                throw ex;
            }
        
            if (!string.IsNullOrEmpty(token))
            {
                await _projectNotificationsService.SendNotificationSuccessDeleteProjectAsync(
                    "Все хорошо",
                    "Проект успешно удален.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);  
            }

            var user = await _userRepository.GetUserPhoneEmailByUserIdAsync(userId);

            // Отправляем уведомление на почту владельца об удаленном проекте и вакансиях привязанных к проекту.
            await _mailingsService.SendNotificationDeleteProjectAsync(user.Email, removedProject.ProjectName,
                removedProject.RemovedVacancies);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список вакансий доступных к отклику.
    /// Для владельца проекта будет возвращаться пустой список.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список вакансий доступных к отклику.</returns>
    public async Task<IEnumerable<ProjectVacancyEntity>> GetAvailableResponseProjectVacanciesAsync(long projectId,
        string account)
    {
        try
        {
            if (projectId <= 0)
            {
                var ex = new ArgumentNullException($"Id проекта не может быть <= 0. ProjectId: {projectId}");
                throw ex;
            }
            
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
        
            // Проверяем, является ли текущий пользователь владельцем проекта.
            var isOwner = await _projectRepository.CheckProjectOwnerAsync(projectId, userId);

            // Владелец не отправляет заявки на отклики на свой же проект.
            if (isOwner)
            {
                return null;
            }

            // Получаем Id владельца проекта.
            var ownerId = await _projectRepository.GetProjectOwnerIdAsync(projectId);

            var result = await _projectRepository.GetAvailableResponseProjectVacanciesAsync(ownerId, projectId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
    
    /// <summary>
    /// Метод добавляет проект в архив.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="token">Токен.</param>
    public async Task AddProjectArchiveAsync(long projectId, string account, string token)
    {
        try
        {
            if (projectId <= 0)
            {
                var ex = new InvalidOperationException($"Id проекта не может быть <= 0. ProjectId: {projectId}");
                throw ex;
            }
            
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            // Проверяем, является ли текущий пользователь владельцем проекта.
            var isOwner = await _projectRepository.CheckProjectOwnerAsync(projectId, userId);

            // Только владелец может добавить проект в архив.
            if (!isOwner)
            {
                throw new InvalidOperationException("Пользователь не является владельцем проекта." +
                                                    "Добавление в архив невозможно." +
                                                    $"ProjectId: {projectId}." +
                                                    $"UserId: {userId}");
            }
            
            // Проверяем, есть ли уже такой проект в архиве.
            var isExists = await _projectRepository.CheckProjectArchiveAsync(projectId);
            
            if (isExists)
            {
                _logger.LogWarning($"Такой проект уже добавлен в архив. ProjectId: {projectId}. UserId: {userId}");

                if (!string.IsNullOrEmpty(token))
                {
                    await _projectNotificationsService.SendNotificationWarningAddProjectArchiveAsync("Внимание",
                        "Такой проект уже добавлен в архив.",
                        NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);
                }
                
                return;
            }

            await _projectRepository.AddProjectArchiveAsync(projectId, userId);
            
            if (!string.IsNullOrEmpty(token))
            {
                await _projectNotificationsService.SendNotificationSuccessAddProjectArchiveAsync("Все хорошо",
                    "Проект успешно добавлен в архив.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
            }
            
            var projectName = await _projectRepository.GetProjectNameByProjectIdAsync(projectId);

            // Отправляем уведомление на почту.
            await _mailingsService.SendNotificationAddProjectArchiveAsync(account, projectId, projectName);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            
            if (!string.IsNullOrEmpty(token))
            {
                await _projectNotificationsService.SendNotificationErrorAddProjectArchiveAsync("Что то не так...",
                    "Ошибка при добавлении проекта в архив. Мы уже знаем о проблеме и уже занимаемся ей.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
            }
            
            throw;
        }
    }

    /// <summary>
    /// Метод удаляет из архива проект.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="token">Токен.</param>
    public async Task DeleteProjectArchiveAsync(long projectId, string account, string token)
    {
        try
        {
            if (projectId <= 0)
            {
                var ex = new InvalidOperationException($"Id проекта не может быть <= 0. ProjectId: {projectId}");
                throw ex;
            }
            
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            // Проверяем, является ли текущий пользователь владельцем проекта.
            var isOwner = await _projectRepository.CheckProjectOwnerAsync(projectId, userId);

            // Только владелец может удалить проект из архива.
            if (!isOwner)
            {
                throw new InvalidOperationException("Пользователь не является владельцем проекта." +
                                                    "Удаление из архива невозможно." +
                                                    $"ProjectId: {projectId}." +
                                                    $"UserId: {userId}");
            }
            
            // Получаем подписку пользователя.
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(userId);
            
            // Получаем тариф, на который оформлена подписка у пользователя.
            var fareRule = await _fareRuleRepository.GetByIdAsync(userSubscription.ObjectId);
            var fareRuleName = fareRule.Name;

            // Проверяем кол-во опубликованных проектов пользователя.
            // Если по лимитам тарифа доступно, то разрешаем удалить проект из архива.
            var projectsCatalogCount = await _projectRepository.GetUserProjectsCatalogCountAsync(userId);

            // Проверяем кол-во в зависимости от подписки.
            // Если стартовый тариф.
            if (fareRuleName.Equals(FareRuleTypeEnum.Start.GetEnumDescription()))
            {
                if (projectsCatalogCount >= AvailableLimitsConst.AVAILABLE_PROJECT_START_COUNT)
                {
                    var ex = new InvalidOperationException(NOT_AVAILABLE_DELETE_PROJECT_ARCHIVE);
                    
                    _logger.LogError(ex, ex.Message);
                    
                    if (!string.IsNullOrEmpty(token))
                    {
                        await _projectNotificationsService.SendNotificationWarningDeleteProjectArchiveAsync("Внимание",
                            NOT_AVAILABLE_DELETE_PROJECT_ARCHIVE, NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING,
                            token);
                    }
                    
                    throw ex;
                }
            }
            
            // Если базовый тариф.
            if (fareRuleName.Equals(FareRuleTypeEnum.Base.GetEnumDescription()))
            {
                if (projectsCatalogCount >= AvailableLimitsConst.AVAILABLE_PROJECT_BASE_COUNT)
                {
                    var ex = new InvalidOperationException(NOT_AVAILABLE_DELETE_PROJECT_ARCHIVE);
                    
                    _logger.LogError(ex, ex.Message);
                    
                    if (!string.IsNullOrEmpty(token))
                    {
                        await _projectNotificationsService.SendNotificationWarningDeleteProjectArchiveAsync("Внимание",
                            NOT_AVAILABLE_DELETE_PROJECT_ARCHIVE, NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING,
                            token);
                    }
                    
                    throw ex;
                }
            }
            
            // Если бизнес тариф.
            if (fareRuleName.Equals(FareRuleTypeEnum.Business.GetEnumDescription()))
            {
                if (projectsCatalogCount >= AvailableLimitsConst.AVAILABLE_PROJECT_BUSINESS_COUNT)
                {
                    var ex = new InvalidOperationException(NOT_AVAILABLE_DELETE_PROJECT_ARCHIVE);
                    
                    _logger.LogError(ex, ex.Message);
                    
                    if (!string.IsNullOrEmpty(token))
                    {
                        await _projectNotificationsService.SendNotificationWarningDeleteProjectArchiveAsync("Внимание",
                            NOT_AVAILABLE_DELETE_PROJECT_ARCHIVE, NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING,
                            token);
                    }
                    
                    throw ex;
                }
            }

            // Удаляем проект из архива.
            var isDelete = await _projectRepository.DeleteProjectArchiveAsync(projectId, userId);

            if (!isDelete && !string.IsNullOrEmpty(token))
            {
                await _projectNotificationsService.SendNotificationErrorDeleteProjectArchiveAsync("Что то не так...",
                    "Ошибка при удалении проекта из архива. Мы уже знаем о проблеме и уже занимаемся ей.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);

                return;
            }
            
            // Отправляем проект на модерацию.
            await _projectModerationRepository.AddProjectModerationAsync(projectId);
            
            if (!string.IsNullOrEmpty(token))
            {
                await _projectNotificationsService.SendNotificationSuccessDeleteProjectArchiveAsync("Все хорошо",
                    "Проект успешно удален из архива.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
            }
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            
            if (!string.IsNullOrEmpty(token))
            {
                await _projectNotificationsService.SendNotificationErrorDeleteProjectArchiveAsync("Что то не так...",
                    "Ошибка при удалении проекта из архива. Мы уже знаем о проблеме и уже занимаемся ей.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
            }
            
            throw;
        }
    }

    /// <summary>
    /// Метод запускает првоерки на разные условия прежде чем вывести проекты в каталог.
    /// Проекты могут быть отсеяны, если не проходят по условиям.
    /// </summary>
    /// <param name="projects">Список проектов до проверки условий.</param>
    /// <returns>Список проектов после проверки условий.</returns>
    public async Task<IEnumerable<CatalogProjectOutput>> ExecuteCatalogConditionsAsync(
        IEnumerable<CatalogProjectOutput> projects)
    {
        var catalogs = projects.ToList();
        
        if (!catalogs.Any())
        {
            return Enumerable.Empty<CatalogProjectOutput>();
        }
        
        await DeleteIfProjectRemarksAsync(catalogs);
            
        // Выбираем пользователей, у которых есть подписка выше бизнеса. Только их выделяем цветом.
        projects = await _fillColorProjectsService.SetColorBusinessProjectsAsync(catalogs, _subscriptionRepository,
            _fareRuleRepository);

        // Очистка описание от тегов список проектов для каталога.
        projects = ClearCatalogVacanciesHtmlTags(projects.ToList());

        // Проставляем проектам теги, в зависимости от подписки владельца проекта.
        projects = await SetProjectsTagsAsync(projects.ToList());
        
        // Исключаем проекты на модерации и архивные.
        // catalogs.ToList().RemoveAll(p => p.IsModeration || p.IsArchived);

        return projects;
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
            var vacancyName = string.Empty;
            
            // Заполняем название вакансии.
            if (member.UserVacancy.VacancyId > 0)
            {
                vacancyName = await _vacancyRepository.GetVacancyNameByVacancyIdAsync(member.UserVacancy.VacancyId);   
            }

            // Если владелец, то у него не обязательно требовать вакансию.
            else if (member.UserVacancy.VacancyId == 0)
            {
                vacancyName = "-";
            }

            if (string.IsNullOrEmpty(vacancyName))
            {
                var ex = new InvalidOperationException(
                    $"Ошибка получения названия вакансии. VacancyId = {member.UserVacancy.VacancyId}");
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
    /// <param name="token">Токен пользователя.</param>
    private async Task ValidateInviteProjectTeamParams(string inviteText, ProjectInviteTypeEnum inviteType,
        long projectId, long? vacancyId, string account, string token)
    {
        var isError = false;

        if (string.IsNullOrEmpty(inviteText))
        {
            var ex = new ArgumentException(ValidationConsts.NOT_VALID_INVITE_PROJECT_TEAM_USER);
            _logger.LogError(ex, ex.Message);
            isError = true;
        }
        
        if (!_projectInviteTypes.Contains(inviteType))
        {
            var ex = new ArgumentException(ValidationConsts.NOT_VALID_INVITE_TYPE);
            _logger.LogError(ex, ex.Message);
            isError = true;
        }

        if (projectId <= 0)
        {
            var ex = new ArgumentException(ValidationConsts.NOT_VALID_INVITE_PROJECT_TEAM_PROJECT_ID);
            _logger.LogError(ex, ex.Message);
            isError = true;
        }

        if (vacancyId <= 0)
        {
            var ex = new ArgumentException(ValidationConsts.NOT_VALID_INVITE_PROJECT_TEAM_VACANCY_ID);
            _logger.LogError(ex, ex.Message);
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
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
        }
    }
    
    /// <summary>
    /// Метод валидирует Id проекта. Выбрасываем исклчюение, если он невалидный.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="token">Токен пользователя.</param>
    private async Task ValidateProjectIdAsync(long projectId, string token)
    {
        var ex = new ArgumentNullException(string.Concat(ValidationConsts.NOT_VALID_PROJECT_ID, projectId));
        _logger.LogError(ex, ex.Message);
        
        await _projectNotificationsService.SendNotificationErrorUpdatedUserProjectAsync("Что то не так...",
            "Ошибка при обновлении проекта. Мы уже знаем о проблеме и уже занимаемся ей.",
            NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
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
            result.IsVisibleActionDeleteProjectTeamMember = true;
            
            // Проверяем, есть ли проект в архиве.
            var isExists = await _projectRepository.CheckProjectArchiveAsync(projectId);

            if (!isExists)
            {
                result.IsVisibleActionAddProjectArchive = true;   
            }
        }

        else
        {
            // Просматривает не владелец, допускаем видимость кнопок действий проекта.
            result.IsVisibleActionProjectButtons = true;
            
            // Отображаем кнопку покидания проекта, если участник есть в команде проекта.
            var isExistsProjectTeamMember = await _projectRepository.CheckExistsProjectTeamMemberAsync(projectId,
                userId);

            if (isExistsProjectTeamMember)
            {
                result.IsVisibleActionLeaveProjectTeam = true;   
            }
        }

        result.IsSuccess = true;

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
        return date.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("ru"));
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
    /// <param name="token">Токен пользователя.</param>
    private async Task DisplayNotificationsAfterResponseProjectAsync(long? vacancyId, long responseId, string token)
    {
        if (responseId > 0)
        {
            await _projectNotificationsService.SendNotificationSuccessProjectResponseAsync(
                "Все хорошо",
                "Отклик на проект успешно оставлен. Вы получите уведомление о решении владельца проекта.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
        }

        else
        {
            if (vacancyId > 0)
            {
                await _projectNotificationsService.SendNotificationErrorProjectResponseAsync("Ошибка",
                    "Ошибка при отклике на проект с указанием вакансии. Мы уже знаем о ней и разбираемся. " +
                    "А пока, попробуйте еще раз.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
            }

            else
            {
                await _projectNotificationsService.SendNotificationErrorProjectResponseAsync("Ошибка",
                    "Ошибка при отклике на проект без указания вакансии. Мы уже знаем о ней и разбираемся. " +
                    "А пока, попробуйте еще раз.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
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
        
        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(inviteText);
            throw ex;
        }

        return userId;
    }

    /// <summary>
    /// Метод добавляет владельца проекта в команду своего проекта по дефолту.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    private async Task AddProjectOwnerToTeamMembersAsync(long userId, long projectId)
    {
        var team = await _projectRepository.GetProjectTeamAsync(projectId);

        if (team is null)
        {
            var ex = new InvalidOperationException("Ошибка добавления владельца проекта в команду.");
            _logger.LogError(ex, ex.Message);
            throw ex;
        }
        
        _ = await _projectRepository.AddProjectTeamMemberAsync(userId, null, team.TeamId);
    }
    
    /// <summary>
    /// Метод чистит описание от тегов список проектов для каталога.
    /// </summary>
    /// <param name="projects">Список проектов.</param>
    /// <returns>Список проектов после очистки.</returns>
    private IEnumerable<CatalogProjectOutput> ClearCatalogVacanciesHtmlTags(List<CatalogProjectOutput> projects)
    {
        // Чистим описание проекта от html-тегов.
        foreach (var prj in projects)
        {
            prj.ProjectDetails = ClearHtmlBuilder.Clear(prj.ProjectDetails);
        }

        return projects;
    }

    /// <summary>
    /// Метод отправляет пользователю уведомление на почту о приглашении его в проект.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="inviteUserId">Id пользователя, которого пригласили в проект.</param>
    private async Task SendEmailNotificationInviteTeamProjectAsync(long projectId, long inviteUserId,
        string projectName)
    {
        // Находим данные пользователя.
        var user = await _userRepository.GetUserPhoneEmailByUserIdAsync(inviteUserId);
            
        // Находим почту владельца проекта.
        var projectOwnerEmail = await _projectRepository.GetProjectOwnerEmailByProjectIdAsync(projectId);
        
        await _mailingsService.SendNotificationInviteTeamProjectAsync(user.Email, projectId, projectName,
            projectOwnerEmail);
    }

    /// <summary>
    /// Метод получает список проектов пользователя из архива.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список архивированных проектов.</returns>
    public async Task<UserProjectArchiveResultOutput> GetUserProjectsArchiveAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                throw new NotFoundUserIdByAccountException(account);
            }

            var result = new UserProjectArchiveResultOutput
            {
                ProjectsArchive = new List<ProjectArchiveOutput>()
            };

            // Находим проекты в архиве.
            var archivedProjects = await _projectRepository.GetUserProjectsArchiveAsync(userId);

            var archivedProjectEntities = archivedProjects.ToList();
            
            if (!archivedProjectEntities.Any())
            {
                return result;
            }

            result.ProjectsArchive = _mapper.Map<List<ProjectArchiveOutput>>(archivedProjects);
            
            await CreateProjectsDatesHelper.CreateDatesResultAsync(archivedProjectEntities,
                result.ProjectsArchive.ToList());

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод удаляет участника проекта из команды.
    /// </summary>
    /// <param name="projectId">Id проекта</param>
    /// <param name="userId">Id пользователя, которого будем удалять из команды</param>
    /// <param name="token">Токен.</param>
    public async Task DeleteProjectTeamMemberAsync(long projectId, long userId, string token)
    {
        try
        {
            if (projectId <= 0)
            {
                throw new ArgumentNullException(
                    $"Не передан Id проекта для удаления из команды. ProjectId: {projectId}");
            }

            if (userId <= 0)
            {
                throw new ArgumentNullException(
                    $"Не передан Id пользователя для удаления из команды. UserId: {userId}");
            }

            // Находим Id команды проекта.
            var projectTeamId = await _projectRepository.GetProjectTeamIdAsync(projectId);
            
            // Удаляем участника команды проекта.
            await _projectRepository.DeleteProjectTeamMemberAsync(userId, projectTeamId);

            if (!string.IsNullOrEmpty(token))
            {
                await _projectNotificationsService.SendNotificationSuccessDeleteProjectTeamMemberAsync("Все хорошо",
                    "Пользователь исключен из команды проекта.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
            }
            
            var projectName = await _projectRepository.GetProjectNameByProjectIdAsync(projectId);
            
            // Записываем уведомления о исключении из команды проекта.
            await _projectNotificationsRepository.AddNotificationDeleteProjectTeamMemberAsync(
                projectId, null, userId, projectName);
            
            // Находим данные о пользователе, который оставляет отклик на проект.
            var otherUser = await _userRepository.GetUserPhoneEmailByUserIdAsync(userId);
            
            // Отправляем уведомление на почту пользователя, которого исключили.
            await _mailingsService.SendNotificationDeleteProjectTeamMemberAsync(otherUser.Email, projectId,
                projectName);
        }
        
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(token))
            {
                await _projectNotificationsService.SendNotificationErrorInviteProjectTeamMembersAsync("Ошибка",
                    "Ошибка при удалении пользователя из команды проекта. Мы уже знаем о ней и разбираемся. " +
                    "А пока, попробуйте еще раз.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
            }

            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод покидания команды проекта.
    /// </summary>
    /// <param name="projectId">Id проекта</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="token">Токен.</param>
    public async Task LeaveProjectTeamAsync(long projectId, string account, string token)
    {
        try
        {
            if (projectId <= 0)
            {
                throw new ArgumentNullException(
                    $"Не передан Id проекта для удаления из команды. ProjectId: {projectId}");
            }

            var userId = await _userRepository.GetUserIdByEmailAsync(account);
            
            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            // Находим Id команды проекта.
            var projectTeamId = await _projectRepository.GetProjectTeamIdAsync(projectId);

            // Удаляем участника команды проекта.
            await _projectRepository.LeaveProjectTeamAsync(userId, projectTeamId);

            if (!string.IsNullOrEmpty(token))
            {
                await _projectNotificationsService.SendNotificationSuccessDeleteProjectTeamMemberAsync("Все хорошо",
                    "Вы успешно покинули проект.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
            }

            var projectName = await _projectRepository.GetProjectNameByProjectIdAsync(projectId);

            // Записываем уведомления о исключении из команды проекта.
            await _projectNotificationsRepository.AddNotificationLeaveProjectTeamMemberAsync(projectId, null, userId,
                projectName);
        }
        
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(token))
            {
                await _projectNotificationsService.SendNotificationErrorInviteProjectTeamMembersAsync("Ошибка",
                    "Ошибка при покидании проекта. Мы уже знаем о ней и разбираемся. " +
                    "А пока, попробуйте еще раз.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, token);
            }
            
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список замечаний проекта, если они есть.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список замечаний проекта.</returns>
    public async Task<IEnumerable<ProjectRemarkEntity>> GetProjectRemarksAsync(long projectId, string account)
    {
        if (projectId <= 0)
        {
            var ex = new InvalidOperationException($"Ошибка при получении замечаний проекта. ProjectId: {projectId}");
            throw ex;
        }
        
        var userId = await _userRepository.GetUserIdByEmailAsync(account);
            
        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        var isProjectOwner = await _projectRepository.CheckProjectOwnerAsync(projectId, userId);

        if (!isProjectOwner)
        {
            return Enumerable.Empty<ProjectRemarkEntity>();
        }

        var result = await _projectModerationRepository.GetProjectRemarksAsync(projectId);

        return result;
    }

    /// <summary>
    /// Метод проставляет флаги проектам пользователя в зависимости от его подписки.
    /// </summary>
    /// <param name="projects">Список проектов каталога.</param>
    /// <returns>Список проектов каталога с проставленными тегами.</returns>
    private async Task<IEnumerable<CatalogProjectOutput>> SetProjectsTagsAsync(List<CatalogProjectOutput> projects)
    {
        foreach (var p in projects)
        {
            // Получаем подписку пользователя.
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(p.UserId);

            // Такая подписка не дает тегов.
            if (userSubscription.ObjectId < 3)
            {
                continue;
            }
            
            // Если подписка бизнес.
            if (userSubscription.ObjectId == 3)
            {
                p.TagColor = "warning";
                p.TagValue = "Бизнес";
            }
        
            // Если подписка профессиональный.
            if (userSubscription.ObjectId == 4)
            {
                p.TagColor = "warning";
                p.TagValue = "Профессиональный";
            }   
        }

        return projects;
    }

    /// <summary>
    /// Метод удаляет из результата проекты, которые не попадут в каталог из-за замечаний.
    /// </summary>
    /// <param name="projects">Список проектов.</param>
    private async Task DeleteIfProjectRemarksAsync(List<CatalogProjectOutput> projects)
    {
        var removedProjects = new List<CatalogProjectOutput>();
            
        // Исключаем проекты, которые имеют неисправленные замечания.
        foreach (var prj in projects)
        {
            var isRemarks = await _projectModerationRepository.GetProjectRemarksAsync(prj.ProjectId);
                
            if (!isRemarks.Any())
            {
                continue;
            }
                
            removedProjects.Add(prj);
        }

        if (removedProjects.Any())
        {
            projects.RemoveAll(p => removedProjects.Select(x => x.ProjectId).Contains(p.ProjectId));
        }
    }

    /// <summary>
    /// Метод обновляет статус замечаниям на статус "На проверке", если есть неисправленные.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    private async Task CheckAwaitingCorrectionRemarksAsync(long projectId)
    {
        var remarks = await _projectModerationRepository.GetProjectRemarksAsync(projectId);

        if (!remarks.Any())
        {
            return;
        }

        var awaitingRemarks = new List<ProjectRemarkEntity>();
        
        foreach (var r in remarks)
        {
            if (r.RemarkStatusId != (int)RemarkStatusEnum.AwaitingCorrection)
            {
                continue;
            }

            r.RemarkStatusId = (int)RemarkStatusEnum.Review;
            awaitingRemarks.Add(r);
        }

        if (awaitingRemarks.Any())
        {
            await _projectModerationRepository.UpdateProjectRemarksAsync(awaitingRemarks);
        }
    }
    
    #endregion
}