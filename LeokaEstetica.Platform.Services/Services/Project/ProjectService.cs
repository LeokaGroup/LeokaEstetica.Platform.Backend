using AutoMapper;
using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Core.Helpers;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Services.Abstractions.Project;
using LeokaEstetica.Platform.Services.Validators;

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
    
    /// <summary>
    /// Если Id проекта невалидный.
    /// </summary>
    private const string NOT_VALID_PROJECT_ID = "Невалидный Id проекта. ProjectId был ";
    
    public ProjectService(IProjectRepository projectRepository, 
        ILogService logService, 
        IUserRepository userRepository, 
        IMapper mapper,
        IProjectNotificationsService projectNotificationsService)
    {
        _projectRepository = projectRepository;
        _logService = logService;
        _userRepository = userRepository;
        _mapper = mapper;
        _projectNotificationsService = projectNotificationsService;
    }

    /// <summary>
    /// Метод создает новый проект пользователя.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="projectStage">Стадия проекта.</param>
    /// <returns>Данные нового проекта.</returns>
    public async Task<CreateProjectOutput> CreateProjectAsync(string projectName, string projectDetails, string account, ProjectStageEnum projectStage)
    {
        try
        {
            var result = new CreateProjectOutput();
            
            if (string.IsNullOrEmpty(account))
            {
                var ex = new ArgumentNullException($"Не передан аккаунт пользователя.");
                await _logService.LogErrorAsync(ex);
            }
            
            ProjectValidator.ValidateCreateProject(projectName, projectDetails, ref result);

            if (result.Errors.Any())
            {
                result.IsSuccess = false;
                
                return result;
            }

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
                await _projectNotificationsService.SendNotificationWarningDublicateUserProjectAsync("Увы...", "Такой проект у вас уже существует.", NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING);
                result.IsSuccess = false;
                
                return result;
            }
            
            var statusId = ProjectStatus.GetProjectStatusIdBySysName(ProjectStatusNameEnum.Moderation.ToString());
            var statusName = ProjectStatus.GetProjectStatusNameBySysName(ProjectStatusNameEnum.Moderation.ToString());
            var project = await _projectRepository.CreateProjectAsync(projectName, projectDetails, userId, ProjectStatusNameEnum.Moderation.ToString(), statusId, statusName, projectStage);
                
            // Если что то пошло не так при создании проекта.
            if (project?.ProjectId <= 0)
            {
                var ex = new Exception("Ошибка при создании проекта.");
                await _logService.LogErrorAsync(ex);
                await _projectNotificationsService.SendNotificationErrorCreatedUserProjectAsync("Что то пошло не так", "Ошибка при создании проекта. Мы уже знаем о проблеме и уже занимаемся ей.", NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR);
                
                result.IsSuccess = false;
                
                return result;
            }

            result = _mapper.Map<CreateProjectOutput>(project);

            // Отправляем уведомление об успешном создании проекта.
            await _projectNotificationsService.SendNotificationSuccessCreatedUserProjectAsync("Все хорошо", "Данные успешно сохранены. Проект отправлен на модерацию.", NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS);
            result.IsSuccess = true;

            return result;
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
    public async Task<UpdateProjectOutput> UpdateProjectAsync(string projectName, string projectDetails, string account, long projectId, ProjectStageEnum projectStage)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);
            
            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                await _logService.LogErrorAsync(ex);
                await _projectNotificationsService.SendNotificationErrorUpdatedUserProjectAsync("Что то не так...", "Ошибка при обновлении проекта. Мы уже знаем о проблеме и уже занимаемся ей.", NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR);
                throw ex;
            }
            
            if (projectId <= 0)
            {
                await ValidateProjectIdAsync(projectId);
            }

            var result = new UpdateProjectOutput();
            ProjectValidator.ValidateUpdateProject(projectName, projectDetails, ref result);
            
            if (result.Errors.Any())
            {
                result.IsSuccess = false;
                
                return result;
            }
            
            // Изменяем проект в БД.
            result = await _projectRepository.UpdateProjectAsync(projectName, projectDetails, userId, projectId, projectStage);
            
            // TODO: Добавить отправку проекта на модерацию тут. Также удалять проект из каталога проектов на время модерации.
            await _projectNotificationsService.SendNotificationSuccessUpdatedUserProjectAsync("Все хорошо", "Данные успешно изменены. Проект отправлен на модерацию.", NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS);

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
            var result = new ProjectOutput();
            ProjectValidator.ValidateGetProject(projectId, mode, ref result);

            if (result.Errors.Any())
            {
                result.IsSuccess = false;
                
                return result;
            }
            
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

            var project = await _projectRepository.GetProjectAsync(projectId, userId);

            if (project is null)
            {
                var ex = new NullReferenceException($"Не удалось найти проект с ProjectId {projectId} и UserId {userId}");
                await _logService.LogErrorAsync(ex);
            }

            result = _mapper.Map<ProjectOutput>(project);

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
        };
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
    public async Task<ProjectVacancyResultOutput> ProjectVacanciesAsync(long projectId)
    {
        try
        {
            var result = new ProjectVacancyResultOutput();
            
            if (projectId <= 0)
            {
                await ValidateProjectIdAsync(projectId);
            }

            var items = await _projectRepository.ProjectVacanciesAsync(projectId);

            if (items.Any())
            {
                result.ProjectVacancies = _mapper.Map<IEnumerable<ProjectVacancyOutput>>(items);
            }

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}