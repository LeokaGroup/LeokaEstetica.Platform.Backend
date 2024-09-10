using AutoMapper;
using Dapper;
using LeokaEstetica.Platform.Access.Abstractions.ProjectManagement;
using LeokaEstetica.Platform.Access.Abstractions.User;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Extensions.HtmlExtensions;
using LeokaEstetica.Platform.CallCenter.Abstractions.Vacancy;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Notification;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Database.Abstractions.Subscription;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectTeam;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.ProjectTeam;
using LeokaEstetica.Platform.Models.Entities.User;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using LeokaEstetica.Platform.Services.Abstractions.Vacancy;
using LeokaEstetica.Platform.Services.Consts;
using LeokaEstetica.Platform.Services.Strategies.Project.Team;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Project;
using LeokaEstetica.Platform.Database.MongoDb.Abstractions;
using LeokaEstetica.Platform.Integrations.Abstractions.Discord;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Services.Helpers;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Models.Dto.Output.Orders;

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

    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IMailingsService _mailingsService;
    private static readonly string _archiveVacancy = "В архиве";
    private readonly IGlobalConfigRepository _globalConfigRepository;
    private readonly IProjectManagmentRepository _projectManagmentRepository;
    private readonly IWikiTreeRepository _wikiTreeRepository;

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
    private readonly IAccessUserService _accessUserService;
    private readonly IProjectModerationRepository _projectModerationRepository;

    private readonly IDiscordService _discordService;
    private readonly IProjectManagementSettingsRepository _projectManagementSettingsRepository;

    /// <summary>
    /// Id вакансий, которые будут удалены из результата.
    /// </summary>
    private readonly List<long> _removedVacancyIds = new();

    private readonly IAccessModuleService _accessModuleService;
    private readonly Lazy<IHubNotificationService> _hubNotificationService;
    private readonly IMongoDbRepository _mongoDbRepository;

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
    /// <param name="vacancyModerationService">Сервис модерации вакансий проектов.</param>
    /// <param name="notificationsRepository">Репозиторий уведомлений.</param>
    /// <param name="accessUserService">Сервис доступа пользователя.</param>
    /// <param name="projectModerationRepository">Репозиторий модерации проектов.</param>
    /// <param name="discordService">Сервис уведомлений дискорда.</param>
    /// <param name="projectManagementSettingsRepository">Репозиторий настроек проекта.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    /// <param name="projectManagmentRepository">Репозиторий модуля УП.</param>
    /// <param name="wikiTreeRepository">Репозиторий Wiki модуля УП.</param>
    /// <param name="accessModuleService">Сервис проверки доступов.</param>
    /// <param name="hubNotificationService">Сервис уведомлений хабов.</param>
    /// <param name="mongoDbRepository">Репозиторий MongoDB.</param>
    public ProjectService(IProjectRepository projectRepository,
        ILogger<ProjectService> logger,
        IUserRepository userRepository,
        IMapper mapper,
        IProjectNotificationsService projectNotificationsService,
        IVacancyService vacancyService,
        IVacancyRepository vacancyRepository,
        ISubscriptionRepository subscriptionRepository,
        IVacancyModerationService vacancyModerationService,
        IProjectNotificationsRepository projectNotificationsRepository,
        IAccessUserService accessUserService,
        IMailingsService mailingsService,
        IProjectModerationRepository projectModerationRepository,
        IDiscordService discordService,
        IProjectManagementSettingsRepository projectManagementSettingsRepository,
        IGlobalConfigRepository globalConfigRepository,
        IProjectManagmentRepository projectManagmentRepository,
        IWikiTreeRepository wikiTreeRepository,
        IAccessModuleService accessModuleService,
        Lazy<IHubNotificationService> hubNotificationService,
         IMongoDbRepository mongoDbRepository)
    {
        _projectRepository = projectRepository;
        _logger = logger;
        _userRepository = userRepository;
        _mapper = mapper;
        _projectNotificationsService = projectNotificationsService;
        _vacancyService = vacancyService;
        _vacancyRepository = vacancyRepository;
        _subscriptionRepository = subscriptionRepository;
        _vacancyModerationService = vacancyModerationService;
        _projectNotificationsRepository = projectNotificationsRepository;
        _accessUserService = accessUserService;
        _mailingsService = mailingsService;
        _projectModerationRepository = projectModerationRepository;
        _discordService = discordService;
        _projectManagementSettingsRepository = projectManagementSettingsRepository;
        _globalConfigRepository = globalConfigRepository;
        _projectManagmentRepository = projectManagmentRepository;
        _wikiTreeRepository = wikiTreeRepository;
        _accessModuleService = accessModuleService;
        _hubNotificationService = hubNotificationService;
        _mongoDbRepository = mongoDbRepository;
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

            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            // Если нет доступа, то не даем оплатить платный тариф.
            if (isEmptyProfile)
            {
                var ex = new InvalidOperationException($"Анкета пользователя не заполнена. UserId был: {userId}");

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    "Для создания проекта должна быть заполнена информация вашей анкеты.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningEmptyUserProfile",
                    userCode, UserConnectionModuleEnum.Main);

                throw ex;
            }

            // Получаем подписку пользователя.
            var userSubscription = await _subscriptionRepository.GetUserSubscriptionAsync(userId);

            if (userSubscription is null)
            {
                throw new InvalidOperationException("Найдена невалидная подписка пользователя. " +
                                                    $"UserId: {userId}. " +
                                                    "Подписка была NULL или невалидная." +
                                                    $"#1 Ошибка в {nameof(ProjectService)}");
            }

            var projectName = createProjectInput.ProjectName;

            // Проверяем существование такого проекта у текущего пользователя.
            var isCreatedProject = await _projectRepository.CheckCreatedProjectByProjectNameAsync(projectName, userId);

            // Есть дубликат, нельзя создать проект.
            if (isCreatedProject)
            {
                var ex = new InvalidOperationException($"Попытка создать дубликат проекта. UserId: {userId}");

                await _hubNotificationService.Value.SendNotificationAsync("Увы...",
                    "Такой проект у вас уже существует.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningDublicateUserProject",
                    userCode, UserConnectionModuleEnum.Main);

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

                await _hubNotificationService.Value.SendNotificationAsync("Что то пошло не так",
                    "Ошибка при создании проекта. Мы уже знаем о проблеме и уже занимаемся ей.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorCreatedUserProject",
                    userCode, UserConnectionModuleEnum.Main);

                throw ex;
            }

            // Добавляем владельца в участники проекта по дефолту.
            await AddProjectOwnerToTeamMembersAsync(userId, projectId);

            var isEnabledConfigureProjectScrumSettings = await _globalConfigRepository.GetValueByKeyAsync<bool>(
                GlobalConfigKeys.ProjectManagment.PROJECT_MANAGEMENT_CONFIGURE_PROJECT_SCRUM_SETTINGS);

            if (isEnabledConfigureProjectScrumSettings)
            {
                // Заводим Scrum настройки для проекта.
                await _projectManagementSettingsRepository.ConfigureProjectScrumSettingsAsync(projectId);
            }

            var ifExistsCompany = await _projectManagmentRepository.IfExistsCompanyByOwnerIdAsync(userId);

            long companyId; 

            // Сначала создаем компанию, затем добавляем в нее проект.
            if (!ifExistsCompany)
            {
                // Заводим компанию, если она не существует.
                companyId = await _projectManagmentRepository.CreateCompanyAsync(userId);

                // Добавляем текущего пользователи в участники компании с ролью владельца.
                await _projectManagmentRepository.AddCompanyMemberAsync(companyId, userId, "Владелец");

                // Заводим роли для компании.
                await _projectManagementSettingsRepository.AddCompanyMemberRolesAsync(companyId, userId);
            }

            // Если компания существует, то добавляем этот проект в компанию.
            else
            {
                var isCompanyOwner = await _projectManagmentRepository.CheckCompanyOwnerByUserIdAsync(userId);

                if (!isCompanyOwner)
                {
                    throw new InvalidOperationException("Пользователь не является владельцем никакой компании. " +
                                                        $"UserId: {userId}.");
                }

                companyId = await _projectManagmentRepository.GetCompanyIdByOwnerIdAsync(userId);
            }

            // Добавляем новый проект в общее пространство компании.
            await _projectManagmentRepository.AddProjectWorkSpaceAsync(projectId, companyId);

            // Заводим для проекта wiki и ознакомительную страницу.
            await _wikiTreeRepository.CreateProjectWikiAsync(projectId, userId, projectName);

            // Отправляем уведомление об успешном создании проекта.
            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Данные успешно сохранены. Проект отправлен на модерацию.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessCreatedUserProject",
                userCode, UserConnectionModuleEnum.Main);

            var user = await _userRepository.GetUserPhoneEmailByUserIdAsync(userId);

            // Отправляем уведомление о созданном проекте владельцу проекта.
            await _mailingsService.SendNotificationCreatedProjectAsync(user.Email, projectName, projectId);

            // Отправляем уведомление об отправленном проекте на модерацию.
            await _discordService.SendNotificationCreatedProjectBeforeModerationAsync(projectId);

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

            // Сокращаем длину строки описания проекта.
            foreach (var prj in result.UserProjects)
            {
                // Полное описание проекта для тултипа.
                prj.ProjectDetailsTooltip = prj.ProjectDetails;

                // Чистим описание проекта от html-тегов и урезаем строку, если она более 40 символов.
                if (prj.ProjectDetails?.Trim().Length > 40)
                {
                    prj.ProjectDetails = string.Concat(prj.ProjectDetails[..40], "...");
                    prj.ProjectDetails = ClearHtmlBuilder.Clear(prj.ProjectDetails);
                }
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
    public async Task<CatalogProjectResultOutput> GetCatalogProjectsAsync(CatalogProjectInput catalogProjectInput)
    {
        try
        {
            var result = await _projectRepository.GetCatalogProjectsAsync(catalogProjectInput);

            // TODO: Делать все это в запросе метода GetCatalogProjectsAsync.
            result.CatalogProjects = await ExecuteCatalogConditionsAsync(result.CatalogProjects.AsList());

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
            var userId = await _userRepository.GetUserByEmailAsync(account);
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                await _hubNotificationService.Value.SendNotificationAsync("Что то не так...",
                    "Ошибка при обновлении проекта. Мы уже знаем о проблеме и уже занимаемся ей.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorUpdatedUserProject",
                    userCode, UserConnectionModuleEnum.Main);
                throw ex;
            }

            var projectId = updateProjectInput.ProjectId;

            if (projectId <= 0)
            {
                await ValidateProjectIdAsync(projectId.Value, userCode);
            }

            updateProjectInput.UserId = userId;

            // Изменяем проект в БД.
            var result = await _projectRepository.UpdateProjectAsync(updateProjectInput);

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Данные успешно изменены. Проект отправлен на модерацию.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessUpdatedUserProject",
                userCode, UserConnectionModuleEnum.Main);

            // Проверяем наличие неисправленных замечаний.
            await CheckAwaitingCorrectionRemarksAsync(projectId.Value);

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

            var projectModeration = await _projectModerationRepository.GetProjectModerationAsync(projectId);

            //Проверка, чтобы проект не был на модерации или в архиве - если ссылку вбил не владелец проекта.
            if (!isOwner && projectModeration != null &&
                (projectModeration.ModerationStatusId == (int)ProjectModerationStatusEnum.ModerationProject ||
                projectModeration.ModerationStatusId == (int)ProjectModerationStatusEnum.ArchivedProject))
            {
                return new ProjectOutput
                {
                    IsAccess = false,
                    IsSuccess = false,
                    ProjectRemarks = new List<ProjectRemarkOutput>()
                };
            }
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
                return new ProjectOutput
                {
                    IsAccess = false,
                    IsSuccess = false,
                    ProjectRemarks = new List<ProjectRemarkOutput>()
                };
            }

            var result = await CreateProjectResultAsync(projectId, prj, userId, isOwner);

            var remarks = await _projectModerationRepository.GetProjectRemarksAsync(projectId);
            result.ProjectRemarks = _mapper.Map<IEnumerable<ProjectRemarkOutput>>(remarks);

            await ClearProjectFieldsHtmlTagsAsync(result);

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

            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            if (projectId <= 0)
            {
                await ValidateProjectIdAsync(projectId, userCode);
            }

            var result = new ProjectVacancyResultOutput
            {
                ProjectVacancies = new List<ProjectVacancyOutput>()
            };

            // Проставляем признаки видимости кнопок вакансий проекта.
            result = await FillVisibleControlsProjectVacanciesAsync(result, projectId, userId);

            var vacancies = (await _projectRepository.ProjectVacanciesAsync(projectId))?.AsList();

            if (vacancies is null || vacancies.Count == 0)
            {
                return result;
            }

            // Проставляем вакансиям статусы.
            result.ProjectVacancies = await FillVacanciesStatusesAsync(vacancies, userId,
                projectId);

            // Удаляем вакансии, которые пользователю не нужно видеть в результате.
            vacancies.RemoveAll(x => _removedVacancyIds.Contains(x.VacancyId));

            result.ProjectVacancies = vacancies;

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
    public async Task<CreateOrderOutput> CreateProjectVacancyAsync(CreateProjectVacancyInput createProjectVacancyInput)
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

            // Создаем вакансию и привязываем ее к проекту.
            var createdVacancy = await _vacancyService.CreateVacancyAsync(
                new VacancyInput
                {
                    WorkExperience = createProjectVacancyInput.WorkExperience,
                    Employment = createProjectVacancyInput.Employment,
                    Payment = createProjectVacancyInput.Payment,
                    Account = createProjectVacancyInput.Account,
                    VacancyName = createProjectVacancyInput.VacancyName,
                    VacancyText = createProjectVacancyInput.VacancyText,
                    VacancyId = createProjectVacancyInput.VacancyId,
                    ProjectId = createProjectVacancyInput.ProjectId,
                    Demands = createProjectVacancyInput.Demands,
                    Conditions = createProjectVacancyInput.Conditions,
                    UserId = createProjectVacancyInput.UserId
                });

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
    public async Task AttachProjectVacancyAsync(long projectId, long vacancyId, string account)
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

            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            if (isDublicate)
            {
                var ex = new DublicateProjectVacancyException();
                await _hubNotificationService.Value.SendNotificationAsync("Что то не так...",
                    ValidationConst.ProjectVacancy.DUBLICATE_PROJECT_VACANCY,
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR,
                    "SendNotificationErrorDublicateAttachProjectVacancy", userCode, UserConnectionModuleEnum.Main);
                throw ex;
            }

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Вакансия успешно привязана к проекту.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessAttachProjectVacancy",
                userCode, UserConnectionModuleEnum.Main);
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
    /// <returns>Выходная модель с записанным откликом.</returns>
    public async Task<ProjectResponseEntity> WriteProjectResponseAsync(long projectId, long? vacancyId, string account)
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
            // Проверяем заполнение анкеты и даем доступ либо нет.
            var isEmptyProfile = await _accessUserService.IsProfileEmptyAsync(userId);

            // Если нет доступа, то не даем оплатить платный тариф.
            if (isEmptyProfile)
            {
                var ex = new InvalidOperationException($"Анкета пользователя не заполнена. UserId был: {userId}");

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    "Для отклика на проект должна быть заполнена информация вашей анкеты.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningEmptyUserProfile",
                    userCode, UserConnectionModuleEnum.Main);

                throw ex;
            }

            var result = await _projectRepository.WriteProjectResponseAsync(projectId, vacancyId, userId);

            // Показываем уведомления.
            await DisplayNotificationsAfterResponseProjectAsync(vacancyId, result.ResponseId, userCode);

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
            await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                "Вы уже откликались на этот проект.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningProjectResponse", userCode,
                UserConnectionModuleEnum.Main);

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
    /// <returns>Выходная модель.</returns>
    public async Task<InviteProjectTeamOutput> InviteProjectTeamAsync(string inviteText,
        ProjectInviteTypeEnum inviteType, long projectId, long? vacancyId, string account)
    {
        try
        {
            await ValidateInviteProjectTeamParams(inviteText, inviteType, projectId, vacancyId, account);

            var userId = await _userRepository.GetUserIdByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(inviteText);
                throw ex;
            }

            // Проверяем нахождение проекта на модерации.
            var isProjectModeration = await _projectRepository.CheckProjectModerationAsync(projectId);
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            // Если он там есть, то не даем пригласить в него.
            if (isProjectModeration)
            {
                var ex = new InvalidOperationException(
                    "Проект еще на модерации. Нельзя пригласить пользователей, пока проект не пройдет модерацию.");

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    "Проект еще на модерации. Нельзя пригласить пользователей, пока проект не пройдет модерацию.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningProjectInviteTeam",
                    userCode, UserConnectionModuleEnum.Main);

                throw ex;
            }

            // Проверяем нахождение проекта в архиве.
            var isProjectArchived = await _projectRepository.CheckProjectArchivedAsync(projectId);

            // Если он там есть, то не даем пригласить в него.
            if (isProjectArchived)
            {
                var ex = new InvalidOperationException(
                    "Проект в архиве. Нельзя пригласить пользователей, если проект в архиве.");

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    "Проект в архиве. Нельзя пригласить пользователей, если проект в архиве.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningProjectInviteTeam",
                    userCode, UserConnectionModuleEnum.Main);

                throw ex;
            }

            // Проверяем тариф пользователя.
            // Если бесплатный, то проверяем лимит сотрудников в команде проекта.
            var access = await _accessModuleService.CheckAccessInviteProjectTeamMemberAsync(projectId, account);

            // Не даем пригласить в команду проекта.
            if (!access.IsAccess)
            {
                return new InviteProjectTeamOutput
                {
                    ForbiddenTitle = access.ForbiddenTitle,
                    ForbiddenText = access.ForbiddenText,
                    FareRuleText = access.FareRuleText,
                    IsAccess = false
                };
            }

            // Получаем Id команды проекта.
            var teamId = await _projectRepository.GetProjectTeamIdAsync(projectId);

            // Находим Id пользователя, которого приглашаем в проект.
            var inviteUserId = await GetUserIdAsync(inviteText, inviteType);

            var isInvitedUser = await _projectRepository.CheckProjectTeamMemberAsync(teamId, inviteUserId);

            // Проверяем, не отправляли ли уже приглашение, чтобы не плодить дубликаты.
            var isSendedInvite = await _projectNotificationsRepository.CheckSendedInviteProjectTeamAsync(inviteUserId,
                projectId);

            // Проверяем, не приглашали ли уже пользователя в команду проекта. Если да, то не даем пригласить повторно.
            if (isInvitedUser || isSendedInvite)
            {
                var ex = new InvalidOperationException("Пользователь уже был приглашен в команду проекта. " +
                                                       $"TeamId: {teamId}. " +
                                                       $"InvitedUserId: {inviteUserId}. " +
                                                       $"UserId: {userId}");

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    "Пользователь уже был добавлен в команду проекта.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING,
                    "SendNotificationWarningUserAlreadyProjectInvitedTeam", userCode, UserConnectionModuleEnum.Main);

                throw ex;
            }

            // Находим название проекта.
            var projectName = await _projectRepository.GetProjectNameByProjectIdAsync(projectId);

            await SendEmailNotificationInviteTeamProjectAsync(projectId, inviteUserId, projectName);

            // Записываем уведомления о приглашении в проект.
            await _projectNotificationsRepository.AddNotificationInviteProjectAsync(projectId, vacancyId, inviteUserId,
                projectName);

            return new ProjectTeamMemberOutput { IsAccess = true };
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

            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            if (!isRemoved)
            {
                var ex = new InvalidOperationException(
                    "Ошибка удаления вакансии проекта. " +
                    $"VacancyId: {vacancyId}. " +
                    $"ProjectId: {projectId}. " +
                    $"UserId: {userId}");

                await _hubNotificationService.Value.SendNotificationAsync("Ошибка",
                    "Ошибка при удалении вакансии проекта.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorDeleteProjectVacancy",
                    userCode, UserConnectionModuleEnum.Main);

                throw ex;
            }

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Вакансия успешно удалена из проекта.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessDeleteProjectVacancy",
                userCode, UserConnectionModuleEnum.Main);
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
    public async Task DeleteProjectAsync(long projectId, string account)
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

            var removedProject = await _projectRepository.RemoveProjectAsync(projectId, userId);
            var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

            if (!removedProject.Success)
            {
                var ex = new InvalidOperationException(
                    "Ошибка удаления проекта. " +
                    $"ProjectId: {projectId}. " +
                    $"UserId: {userId}");

                await _hubNotificationService.Value.SendNotificationAsync("Ошибка",
                    "Ошибка при удалении проекта.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorDeleteProject",
                    userCode, UserConnectionModuleEnum.Main);

                throw ex;
            }

            // Удаляем документы проекта.
            var mongoDocumentIds = await _projectManagmentRepository.GetProjectMongoDocumentIdsByProjectIdAsync(
                projectId);

            foreach (var did in mongoDocumentIds)
            {
                await _mongoDbRepository.RemoveFileAsync(did);
            }

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Проект успешно удален.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessDeleteProject", userCode,
                UserConnectionModuleEnum.Main);

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
    public async Task AddProjectArchiveAsync(long projectId, string account)
    {
        var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

        try
        {
            if (projectId <= 0)
            {
                var ex = new InvalidOperationException($"Id проекта не может быть <= 0. ProjectId: {projectId}");
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

                await _hubNotificationService.Value.SendNotificationAsync("Внимание",
                    "Такой проект уже добавлен в архив.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, "SendNotificationWarningAddProjectArchive",
                    userCode, UserConnectionModuleEnum.Main);

                return;
            }

            await _projectRepository.AddProjectArchiveAsync(projectId, userId);

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Проект успешно добавлен в архив.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessAddProjectArchive",
                userCode, UserConnectionModuleEnum.Main);

            var projectName = await _projectRepository.GetProjectNameByProjectIdAsync(projectId);

            // Отправляем уведомление на почту.
            await _mailingsService.SendNotificationAddProjectArchiveAsync(account, projectId, projectName);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            await _hubNotificationService.Value.SendNotificationAsync("Что то не так...",
                "Ошибка при добавлении проекта в архив. Мы уже знаем о проблеме и уже занимаемся ей.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorAddProjectArchive", userCode,
                UserConnectionModuleEnum.Main);

            throw;
        }
    }

    /// <summary>
    /// Метод удаляет из архива проект.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    public async Task DeleteProjectArchiveAsync(long projectId, string account)
    {
        var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

        try
        {
            if (projectId <= 0)
            {
                var ex = new InvalidOperationException($"Id проекта не может быть <= 0. ProjectId: {projectId}");
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

            if (userSubscription is null)
            {
                throw new InvalidOperationException("Найдена невалидная подписка пользователя. " +
                                                    $"UserId: {userId}. " +
                                                    "Подписка была NULL или невалидная." +
                                                    $"#2 Ошибка в {nameof(ProjectService)}");
            }

            // Удаляем проект из архива.
            var isDelete = await _projectRepository.DeleteProjectArchiveAsync(projectId, userId);

            if (!isDelete)
            {
                await _hubNotificationService.Value.SendNotificationAsync("Что то не так...",
                    "Ошибка при удалении проекта из архива. Мы уже знаем о проблеме и уже занимаемся ей.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorDeleteProjectArchive",
                    userCode, UserConnectionModuleEnum.Main);

                return;
            }

            // Отправляем проект на модерацию.
            await _projectModerationRepository.AddProjectModerationAsync(projectId);

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Проект успешно удален из архива.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessDeleteProjectArchive",
                userCode, UserConnectionModuleEnum.Main);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            await _hubNotificationService.Value.SendNotificationAsync("Что то не так...",
                "Ошибка при удалении проекта из архива. Мы уже знаем о проблеме и уже занимаемся ей.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorDeleteProjectArchive", userCode,
                UserConnectionModuleEnum.Main);

            throw;
        }
    }

    /// <inheritdoc />
    public async Task SetProjectTeamMemberRoleAsync(long userId, string? role, long projectId)
    {
        try
        {
            // Находим Id команды проекта.
            var teamId = await _projectRepository.GetProjectTeamIdAsync(projectId);

            // Назначаем участнику команды проекта роль.
            await _projectRepository.SetProjectTeamMemberRoleAsync(userId, role, teamId);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RemoveUserProjectTeamAsync(long userId, long projectId)
    {
        try
        {
            // Находим Id команды проекта.
            var teamId = await _projectRepository.GetProjectTeamIdAsync(projectId);

            var isExcludeOwner = await _projectRepository.CheckProjectOwnerAsync(projectId, userId);

            if (isExcludeOwner)
            {
                throw new InvalidOperationException("Попытка исключить владельца проекта. " +
                                                    "Пользователь не знает, что произошло. " +
                                                    "Возможно, показали ему кнопку исключения владельца. " +
                                                    "Срочно разобраться и поправить. " +
                                                    "Исключить владельца невозможно. Прервали пользователя.");
            }

            await _projectRepository.RemoveUserProjectTeamAsync(userId, teamId, projectId);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// TODO: Делать все это в запросе метода GetCatalogProjectsAsync.
    /// Метод запускает проверки на разные условия прежде чем вывести проекты в каталог.
    /// Проекты могут быть отсеяны, если не проходят по условиям.
    /// </summary>
    /// <param name="projects">Список проектов до проверки условий.</param>
    /// <returns>Список проектов после проверки условий.</returns>
    private async Task<IEnumerable<CatalogProjectOutput>> ExecuteCatalogConditionsAsync(
        List<CatalogProjectOutput> projects)
    {
        if (projects.Count == 0)
        {
            return Enumerable.Empty<CatalogProjectOutput>();
        }

        await DeleteIfProjectRemarksAsync(projects);

        // Очистка описания от тегов список проектов для каталога.
        await ClearCatalogVacanciesHtmlTagsAsync(projects.ToList());

        return projects;
    }

    /// <summary>
    /// Метод обновляет видимость проекта
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="isPublic">Видимость проекта.</param>
    /// <returns>Возращает признак видимости проекта.</returns>
    public async Task<UpdateProjectOutput> UpdateVisibleProjectAsync(long projectId, bool isPublic)
    {
        return await _projectRepository.UpdateVisibleProjectAsync(projectId, isPublic);
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
            var isOwner = false;

            // Заполняем название вакансии.
            if (member.UserVacancy.VacancyId > 0)
            {
                vacancyName = await _vacancyRepository.GetVacancyNameByVacancyIdAsync(member.UserVacancy.VacancyId);
            }

            // Если владелец, то у него не обязательно требовать вакансию.
            else if (member.UserVacancy.VacancyId == 0)
            {
                vacancyName = "-";
                isOwner = true;
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

            // Создаем результат команды проекта.
            var team = CreateProjectTeamResult(vacancyName, user, member);

            if (isOwner)
            {
                team.IsVisibleActionDeleteProjectTeamMember = false;
            }

            else
            {
                team.IsVisibleActionDeleteProjectTeamMember = true;
            }

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

        var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

        // Если была ошибка, то покажем уведомление юзеру и генерим исключение.
        if (isError)
        {
            await _hubNotificationService.Value.SendNotificationAsync("Ошибка",
                "Ошибка при добавлении пользователя в команду проекта. Мы уже знаем о ней и разбираемся. " +
                "А пока, попробуйте еще раз.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorInviteProjectTeamMembers",
                userCode, UserConnectionModuleEnum.Main);
        }
    }

    /// <summary>
    /// Метод валидирует Id проекта. Выбрасываем исклчюение, если он невалидный.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userCode">Код пользователя.</param>
    private async Task ValidateProjectIdAsync(long projectId, Guid userCode)
    {
        var ex = new ArgumentNullException(string.Concat(ValidationConsts.NOT_VALID_PROJECT_ID, projectId));
        _logger.LogError(ex, ex.Message);

        await _hubNotificationService.Value.SendNotificationAsync("Что то не так...",
            "Ошибка при обновлении проекта. Мы уже знаем о проблеме и уже занимаемся ей.",
            NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorUpdatedUserProject", userCode,
            UserConnectionModuleEnum.Main);
        throw ex;
    }

    /// <summary>
    /// Метод проставляет статусы вакансиям.
    /// </summary>
    /// <param name="projectVacancies">Список вакансий.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список вакансий.</returns>
    private async Task<IEnumerable<ProjectVacancyOutput>> FillVacanciesStatusesAsync(
        List<ProjectVacancyOutput> projectVacancies, long userId, long projectId)
    {
        // Получаем список вакансий на модерации.
        var moderationVacancies = await _vacancyModerationService.VacanciesModerationAsync();

        // Получаем список вакансий из каталога вакансий.
        var catalogVacancies = await _vacancyRepository.GetCatalogVacanciesAsync(new VacancyCatalogInput());

        // Находим вакансии в архиве.
        var archivedVacancies = (await _vacancyRepository.GetUserVacanciesArchiveAsync(userId)).AsList();

        var isOwner = await _projectRepository.CheckProjectOwnerAsync(projectId, userId);

        // TODO: Когда перепишем на Dapper, то не надо работать с лишними данными, а сразу в запросе отсекать их.
        // TODO: Тогда и список _removedVacancyIds не нужен будет.
        // Проставляем статусы вакансий.
        foreach (var pv in projectVacancies)
        {
            // Ищем в модерации вакансий.
            var isVacancy = moderationVacancies.Vacancies.Any(v => v.VacancyId == pv.VacancyId);

            if (isVacancy)
            {
                pv.VacancyStatusName = moderationVacancies.Vacancies
                    .Where(v => v.VacancyId == pv.VacancyId)
                    .Select(v => v.ModerationStatusName)
                    .FirstOrDefault();

                // Если не владелец, то удаляем из результата вакансии кроме опубликованных.
                if (!isOwner)
                {
                    _removedVacancyIds.Add(pv.VacancyId);
                }
            }

            // Ищем вакансию в каталоге вакансий.
            else
            {
                catalogVacancies.CatalogVacancies ??= new List<CatalogVacancyOutput>();
                var isCatalogVacancy = catalogVacancies.CatalogVacancies.Any(v => v.VacancyId == pv.VacancyId);

                if (isCatalogVacancy)
                {
                    pv.VacancyStatusName = _approveVacancy;
                }
            }

            // Ищем в архиве вакансий.
            var isArchiveVacancy = archivedVacancies.Any(v => v.VacancyId == pv.VacancyId);

            if (isArchiveVacancy)
            {
                pv.VacancyStatusName = _archiveVacancy;
            }

            if (!string.IsNullOrWhiteSpace(pv.VacancyText))
            {
                pv.VacancyText = ClearHtmlBuilder.Clear(pv.VacancyText);
            }
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
        (UserProjectEntity, ProjectStageEntity) prj, long userId, bool isOwner)
    {
        var result = _mapper.Map<ProjectOutput>(prj.Item1);
        result.StageId = prj.Item2.StageId;
        result.StageName = prj.Item2.StageName;
        result.StageSysName = prj.Item2.StageSysName;
        result.IsVisibleProjectButton = isOwner;

        // Проверяем владельца проекта.
        var projectOwnerId = await _projectRepository.GetProjectOwnerIdAsync(projectId);

        // Если владелец проекта, то проставляем признак видимости кнопок событий.
        if (projectOwnerId == userId)
        {
            result.IsVisibleDeleteButton = true;

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
        result.IsAccess = true;

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
            Joined = member.Joined.ToString("d"),
            UserId = member.UserId,
            Role = member.Role
        };

        return team;
    }

    /// <summary>
    /// Метод отправляет уведомления после отклика на проект.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="responseId">Id отклика.</param>
    /// <param name="userCode">Код пользователя.</param>
    private async Task DisplayNotificationsAfterResponseProjectAsync(long? vacancyId, long responseId, Guid userCode)
    {
        if (responseId > 0)
        {
            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Отклик на проект успешно оставлен. Вы получите уведомление о решении владельца проекта.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessProjectResponse", userCode, UserConnectionModuleEnum.Main);
        }

        else
        {
            if (vacancyId > 0)
            {
                await _hubNotificationService.Value.SendNotificationAsync("Ошибка",
                    "Ошибка при отклике на проект с указанием вакансии. Мы уже знаем о ней и разбираемся. " +
                    "А пока, попробуйте еще раз.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorProjectResponse", userCode,
                    UserConnectionModuleEnum.Main);
            }

            else
            {
                await _hubNotificationService.Value.SendNotificationAsync("Ошибка",
                    "Ошибка при отклике на проект без указания вакансии. Мы уже знаем о ней и разбираемся. " +
                    "А пока, попробуйте еще раз.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorProjectResponse", userCode,
                    UserConnectionModuleEnum.Main);
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
                new ProjectInviteTeamLinkStrategy(_userRepository, _projectNotificationsService,
                    _hubNotificationService), inviteText),

            ProjectInviteTypeEnum.Email => await projectInviteTeamJob.GetUserIdAsync(
                new ProjectInviteTeamEmailStrategy(_userRepository, _projectNotificationsService,
                    _hubNotificationService), inviteText),

            ProjectInviteTypeEnum.PhoneNumber => await projectInviteTeamJob.GetUserIdAsync(
                new ProjectInviteTeamPhoneNumberStrategy(_userRepository, _projectNotificationsService,
                    _hubNotificationService), inviteText),

            ProjectInviteTypeEnum.Login => await projectInviteTeamJob.GetUserIdAsync(
                new ProjectInviteTeamLoginStrategy(_userRepository, _projectNotificationsService,
                    _hubNotificationService), inviteText),

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

        _ = await _projectRepository.AddProjectTeamMemberAsync(userId, null, team.TeamId, "Владелец");
    }

    /// <summary>
    /// Метод чистит описание от тегов список проектов для каталога.
    /// </summary>
    /// <param name="projects">Список проектов.</param>
    /// <returns>Список проектов после очистки.</returns>
    private async Task ClearCatalogVacanciesHtmlTagsAsync(List<CatalogProjectOutput> projects)
    {
        // Чистим описание проекта от html-тегов.
        foreach (var prj in projects)
        {
            prj.ProjectDetails = ClearHtmlBuilder.Clear(prj.ProjectDetails);
        }

        await Task.CompletedTask;
    }


    /// <summary>
    /// Метод чистит поля проекта от html-тегов.
    /// </summary>
    /// <param name="project">Данные проекта.</param>
    /// <returns>Данные проекта после очистки.</returns>
    private async Task ClearProjectFieldsHtmlTagsAsync(ProjectOutput project)
    {
        if (!string.IsNullOrWhiteSpace(project.ProjectDetails))
        {
            project.ProjectDetails = ClearHtmlBuilder.Clear(project.ProjectDetails).Trim();
        }

        if (!string.IsNullOrWhiteSpace(project.Demands))
        {
            project.Demands= ClearHtmlBuilder.Clear(project.Demands).Trim();
        }

        if (!string.IsNullOrWhiteSpace(project.Conditions))
        {
            project.Conditions= ClearHtmlBuilder.Clear(project.Conditions).Trim();
        }

        await Task.CompletedTask;
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

            var archivedProjectEntities = archivedProjects.AsList();

            if (!archivedProjectEntities.Any())
            {
                return result;
            }

            result.ProjectsArchive = _mapper.Map<List<ProjectArchiveOutput>>(archivedProjects);

            await CreateProjectsDatesHelper.CreateDatesResultAsync(archivedProjectEntities,
                result.ProjectsArchive.AsList());

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
    public async Task DeleteProjectTeamMemberAsync(long projectId, long userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentNullException(
                $"Не передан Id пользователя для удаления из команды. UserId: {userId}");
        }

        var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

        try
        {
            if (projectId <= 0)
            {
                throw new ArgumentNullException(
                    $"Не передан Id проекта для удаления из команды. ProjectId: {projectId}");
            }

            // Находим Id команды проекта.
            var projectTeamId = await _projectRepository.GetProjectTeamIdAsync(projectId);

            // Удаляем участника команды проекта.
            await _projectRepository.DeleteProjectTeamMemberAsync(userId, projectTeamId);

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Пользователь исключен из команды проекта.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessDeleteProjectTeamMember",
                userCode, UserConnectionModuleEnum.Main);

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
            await _hubNotificationService.Value.SendNotificationAsync("Ошибка",
                "Ошибка при удалении пользователя из команды проекта. Мы уже знаем о ней и разбираемся. " +
                "А пока, попробуйте еще раз.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorInviteProjectTeamMembers",
                userCode, UserConnectionModuleEnum.Main);

            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод покидания команды проекта.
    /// </summary>
    /// <param name="projectId">Id проекта</param>
    /// <param name="account">Аккаунт пользователя.</param>
    public async Task LeaveProjectTeamAsync(long projectId, string account)
    {
        var userId = await _userRepository.GetUserIdByEmailAsync(account);

        if (userId <= 0)
        {
            var ex = new NotFoundUserIdByAccountException(account);
            throw ex;
        }

        var userCode = await _userRepository.GetUserCodeByUserIdAsync(userId);

        try
        {
            if (projectId <= 0)
            {
                throw new ArgumentNullException(
                    $"Не передан Id проекта для удаления из команды. ProjectId: {projectId}");
            }

            // Находим Id команды проекта.
            var projectTeamId = await _projectRepository.GetProjectTeamIdAsync(projectId);

            // Удаляем участника команды проекта.
            await _projectRepository.LeaveProjectTeamAsync(userId, projectTeamId);

            await _hubNotificationService.Value.SendNotificationAsync("Все хорошо",
                "Вы успешно покинули проект.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, "SendNotificationSuccessDeleteProjectTeamMember",
                userCode, UserConnectionModuleEnum.Main);

            var projectName = await _projectRepository.GetProjectNameByProjectIdAsync(projectId);

            // Записываем уведомления о исключении из команды проекта.
            await _projectNotificationsRepository.AddNotificationLeaveProjectTeamMemberAsync(projectId, null, userId,
                projectName);
        }

        catch (Exception ex)
        {
            await _hubNotificationService.Value.SendNotificationAsync("Ошибка",
                "Ошибка при покидании проекта. Мы уже знаем о ней и разбираемся. " +
                "А пока, попробуйте еще раз.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, "SendNotificationErrorInviteProjectTeamMembers",
                userCode, UserConnectionModuleEnum.Main);

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

    /// <summary>
    /// Метод валидирует стадии проекта. Если хоть одну не удалось определить, то логируем такое.
    /// Но не ломаем систему, а просто фиксируем для себя.
    /// </summary>
    /// <param name="projects"></param>
    private async Task ValidateProjectStages(List<CatalogProjectOutput> projects)
    {
        var exeptions = new List<InvalidOperationException>();
        foreach (var p in projects)
        {
            if (p.ProjectStageSysName is null)
            {
                exeptions.Add(new InvalidOperationException(
                    $"Не удалось получить стадию проекта. ProjectId: {p.ProjectId}"));
            }
        }

        if (exeptions.Any())
        {
            var aggEx = new AggregateException("Ошибка при получении стадий проектов.", exeptions);
            _logger.LogError(aggEx, aggEx.Message);

            // Отправляем ошибки в чат пачки.
            await _discordService.SendNotificationErrorAsync(aggEx);
        }
    }

    #endregion
}