using AutoMapper;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Core.Constants;
using LeokaEstetica.Platform.Core.Helpers;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using LeokaEstetica.Platform.Services.Abstractions.Vacancy;

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

    /// <summary>
    /// Если Id проекта невалидный.
    /// </summary>
    private const string NOT_VALID_PROJECT_ID = "Невалидный Id проекта. ProjectId был ";

    public ProjectService(IProjectRepository projectRepository,
        ILogService logService,
        IUserRepository userRepository,
        IMapper mapper,
        IProjectNotificationsService projectNotificationsService,
        IVacancyService vacancyService)
    {
        _projectRepository = projectRepository;
        _logService = logService;
        _userRepository = userRepository;
        _mapper = mapper;
        _projectNotificationsService = projectNotificationsService;
        _vacancyService = vacancyService;
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

            var result = await _projectRepository.GetProjectAsync(projectId, userId);

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
        var ex = new ArgumentNullException(string.Concat(NOT_VALID_PROJECT_ID, projectId));
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
}