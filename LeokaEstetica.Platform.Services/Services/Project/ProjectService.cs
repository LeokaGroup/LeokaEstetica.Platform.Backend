using AutoMapper;
using LeokaEstetica.Platform.Core.Helpers;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
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
    /// <returns>Данные нового проекта.</returns>
    public async Task<CreateProjectOutput> CreateProjectAsync(string projectName, string projectDetails, string account)
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
            var project = await _projectRepository.CreateProjectAsync(projectName, projectDetails, userId, ProjectStatusNameEnum.Moderation.ToString(), statusId, statusName);
                
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
    public async Task<IEnumerable<ColumnNameOutput>> UserProjectsColumnsNamesAsync()
    {
        try
        {
            var items = await _projectRepository.UserProjectsColumnsNamesAsync();

            if (!items.Any())
            {
                throw new NullReferenceException("Не удалось получить поля для таблицы UserProjects.");
            }

            var result = _mapper.Map<IEnumerable<ColumnNameOutput>>(items);

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
    /// <returns>Данные нового проекта.</returns>
    public async Task<UpdateProjectOutput> UpdateProjectAsync(string projectName, string projectDetails, string account, long projectId)
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
            
            if (projectId <= 0)
            {
                var ex = new ArgumentNullException(string.Concat(NOT_VALID_PROJECT_ID, projectId));
                await _logService.LogErrorAsync(ex);
                throw ex;
            }

            var result = new UpdateProjectOutput();
            ProjectValidator.ValidateUpdateProject(projectName, projectDetails, ref result);
            
            if (result.Errors.Any())
            {
                result.IsSuccess = false;
                
                return result;
            }
            
            // Изменяем проект в БД.
            result = await _projectRepository.UpdateProjectAsync(projectName, projectDetails, userId, projectId);
            
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
}