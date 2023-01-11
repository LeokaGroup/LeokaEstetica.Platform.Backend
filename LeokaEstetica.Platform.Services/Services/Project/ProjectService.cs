using AutoMapper;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Helpers;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.LuceneNet.Chains.Project;
using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectTeam;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.ProjectTeam;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using LeokaEstetica.Platform.Services.Abstractions.Vacancy;
using LeokaEstetica.Platform.Services.Builders;
using LeokaEstetica.Platform.Services.Consts;

namespace LeokaEstetica.Platform.Services.Services.Project;

/// <summary>
/// Класс реализует методы сервиса проектов.
/// </summary>
public sealed class ProjectService : IProjectService
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

    public ProjectService(IProjectRepository projectRepository,
        ILogService logService,
        IUserRepository userRepository,
        IMapper mapper,
        IProjectNotificationsService projectNotificationsService,
        IVacancyService vacancyService,
        IVacancyRepository vacancyRepository)
    {
        _projectRepository = projectRepository;
        _logService = logService;
        _userRepository = userRepository;
        _mapper = mapper;
        _projectNotificationsService = projectNotificationsService;
        _vacancyService = vacancyService;
        _vacancyRepository = vacancyRepository;

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
                await _logService.LogErrorAsync(ex);
                throw ex;
            }

            // Проверяем существование такого проекта у текущего пользователя.
            var isCreatedProject = await _projectRepository.CheckCreatedProjectByProjectNameAsync(projectName, userId);

            // Есть дубликат, нельзя создать проект.
            if (isCreatedProject)
            {
                await _projectNotificationsService
                    .SendNotificationWarningDublicateUserProjectAsync("Увы...", "Такой проект у вас уже существует.",
                        NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING);

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
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR);

                return null;
            }

            // Отправляем уведомление об успешном создании проекта.
            await _projectNotificationsService.SendNotificationSuccessCreatedUserProjectAsync("Все хорошо",
                "Данные успешно сохранены. Проект отправлен на модерацию.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS);

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
                throw new NullReferenceException("Не удалось получить поля для таблицы ProjectColumnsNames.");
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
    public async Task<IEnumerable<CatalogProjectOutput>> CatalogProjectsAsync()
    {
        try
        {
            var result = await _projectRepository.CatalogProjectsAsync();

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
                await _logService.LogErrorAsync(ex);
                await _projectNotificationsService.SendNotificationErrorUpdatedUserProjectAsync("Что то не так...",
                    "Ошибка при обновлении проекта. Мы уже знаем о проблеме и уже занимаемся ей.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR);
                throw ex;
            }

            if (projectId <= 0)
            {
                await ValidateProjectIdAsync(projectId);
            }

            // Изменяем проект в БД.
            var result =
                await _projectRepository.UpdateProjectAsync(projectName, projectDetails, userId, projectId,
                    projectStage);

            // TODO: Добавить отправку проекта на модерацию тут. Также удалять проект из каталога проектов на время модерации.
            await _projectNotificationsService.SendNotificationSuccessUpdatedUserProjectAsync("Все хорошо",
                "Данные успешно изменены. Проект отправлен на модерацию.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS);

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
    public async Task<UserProjectEntity> GetProjectAsync(long projectId, ModeEnum mode, string account)
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

            // TODO: Реализовать в будущем.
            // Проверяем, является ли текущий пользователь владельцем проекта.
            // Это защита, если проект изменяется.
            // if (mode.Equals(ModeEnum.Edit.ToString()))
            // {
            //     
            // }

            // TODO: При редактировании давать изменять проект лишь владельцу.
            var result = await _projectRepository.GetProjectAsync(projectId);

            if (result is null)
            {
                var ex = new NullReferenceException(
                    $"Не удалось найти проект с ProjectId {projectId} и UserId {userId}");
                await _logService.LogErrorAsync(ex);
                throw ex;
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

        ;
    }

    /// <summary>
    /// Метод валидирует Id проекта. Выбрасываем исклчюение, если он невалидный.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    private async Task ValidateProjectIdAsync(long projectId)
    {
        var ex = new ArgumentNullException(string.Concat(ValidationConsts.NOT_VALID_PROJECT_ID, projectId));
        await _logService.LogErrorAsync(ex);
        await _projectNotificationsService.SendNotificationErrorUpdatedUserProjectAsync("Что то не так...",
            "Ошибка при обновлении проекта. Мы уже знаем о проблеме и уже занимаемся ей.",
            NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR);
        throw ex;
    }

    /// <summary>
    /// Метод получает список вакансий проекта. Список вакансий, которые принадлежат владельцу проекта.
    /// </summary>
    /// <param name="projectId">Id проекта, вакансии которого нужно получить.</param>
    /// <returns>Список вакансий.</returns>
    public async Task<IEnumerable<ProjectVacancyEntity>> ProjectVacanciesAsync(long projectId)
    {
        try
        {
            if (projectId <= 0)
            {
                await ValidateProjectIdAsync(projectId);
            }

            var result = await _projectRepository.ProjectVacanciesAsync(projectId);

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
            await AttachProjectVacancyAsync(projectId, createdVacancy.VacancyId);

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
    public async Task AttachProjectVacancyAsync(long projectId, long vacancyId)
    {
        try
        {
            var isDublicate = await _projectRepository.AttachProjectVacancyAsync(projectId, vacancyId);

            if (isDublicate)
            {
                var ex = new DublicateProjectVacancyException();
                await _logService.LogErrorAsync(ex);
                await _projectNotificationsService.SendNotificationErrorDublicateAttachProjectVacancyAsync(
                    "Что то не так...",
                    GlobalConfigKeys.ProjectVacancy.DUBLICATE_PROJECT_VACANCY,
                    NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR);
                throw ex;
            }

            await _projectNotificationsService.SendNotificationSuccessAttachProjectVacancyAsync(
                "Все хорошо",
                "Вакансия успешно привязана к проекту.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS);
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
        var result = new ProjectResponseEntity();

        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                await _logService.LogErrorAsync(ex);
                throw ex;
            }

            result = await _projectRepository.WriteProjectResponseAsync(projectId, vacancyId, userId);
            await _projectNotificationsService.SendNotificationSuccessProjectResponseAsync(
                "Все хорошо",
                "Отклик на проект успешно оставлен. Вы получите уведомление о решении владельца проекта.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS);
        }

        catch (DublicateProjectResponseException ex)
        {
            await _projectNotificationsService.SendNotificationWarningProjectResponseAsync(
                "Внимание",
                "Вы уже откликались на этот проект.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING);
            await _logService.LogErrorAsync(ex);
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }

        return result;
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
                var ex = new NullReferenceException($"Команды проекта не найдено. ProjectId = {projectId}");
                await _logService.LogErrorAsync(ex);
                throw ex;
            }

            // Находим участников команды проекта.
            var teamMembers = await _projectRepository.GetProjectTeamMembersAsync(projectTeam.TeamId);

            // Если не нашли участников команды проекта.
            if (teamMembers is null)
            {
                var ex = new NullReferenceException(
                    $"Участников команды проекта не найдено. ProjectId = {projectId}. TeamId = {projectTeam.TeamId}");
                await _logService.LogErrorAsync(ex);
                throw ex;
            }

            var result = await FillMembersDataAsync(teamMembers);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод записывает данные участников команды проекта.
    /// </summary>
    /// <param name="teamMembers">Список участников команды проекта.</param>
    /// <returns>Список с изменениями.</returns>
    private async Task<List<ProjectTeamOutput>> FillMembersDataAsync(IEnumerable<ProjectTeamMemberEntity> teamMembers)
    {
        var result = new List<ProjectTeamOutput>();
        foreach (var member in teamMembers)
        {
            var team = new ProjectTeamOutput();

            // Заполняем название вакансии.
            var vacancyName = await _vacancyRepository.GetVacancyNameByVacancyIdAsync(member.UserVacancy.VacancyId);

            if (string.IsNullOrEmpty(vacancyName))
            {
                var ex = new NullReferenceException(
                    $"Ошибка получения названия вакансии. VacancyId = {member.UserVacancy.VacancyId}");
                await _logService.LogErrorAsync(ex);
                throw ex;
            }

            team.VacancyName = vacancyName;
            var user = await _userRepository.GetUserByUserIdAsync(member.UserId);

            if (user is null)
            {
                var ex = new NullReferenceException(
                    $"Ошибка получения данных пользователя. UserId = {member.UserId}");
                await _logService.LogErrorAsync(ex);
                throw ex;
            }

            // Заполняем участника команды проекта.
            team.Member = CreateProjectTeamMembersBuilder.FillMember(user);

            // Форматируем даты.
            team.Joined = CreateProjectTeamMembersBuilder.Create(member.Joined);
            team.UserId = member.UserId;
            result.Add(team);
        }

        return result;
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
    /// <param name="userName">Пользователь, который будет добавлен в команду проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Добавленный пользователь.</returns>s
    public async Task<ProjectTeamMemberEntity> InviteProjectTeamAsync(string userName, long projectId, long vacancyId)
    {
        try
        {
            await ValidateInviteProjectTeamParams(userName, projectId, vacancyId);

            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(userName);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(userName);
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
                "Ошибка добавления пользователей в команду проекта. " +
                $"User был {userName}. " +
                $"ProjectId был {projectId}. " +
                $"VacancyId был {vacancyId}");
            throw;
        }
    }

    /// <summary>
    /// Метод валидирует входные параметры перед добавлением пользователя в команду проекта.
    /// </summary>
    /// <param name="userId">Id пользователя, который будет добавлен в команду проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    private async Task ValidateInviteProjectTeamParams(string userId, long projectId, long vacancyId)
    {
        var isError = false;

        if (string.IsNullOrEmpty(userId))
        {
            var ex = new ArgumentException(ValidationConsts.NOT_VALID_INVITE_PROJECT_TEAM_USER);
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

        // Если была ошибка, то покажем уведомление юзеру и генерим исключение.
        if (isError)
        {
            await _projectNotificationsService.SendNotificationErrorInviteProjectTeamMembersAsync("Ошибка",
                "Ошибка при добавлении пользователя в команду проекта. Мы уже знаем о ней и разбираемся. " +
                "А пока, попробуйте еще раз.",
                NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR);
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
}