using AutoMapper;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using LeokaEstetica.Platform.Services.Abstractions.Project;

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
    private readonly INotificationsService _notificationsService;
    
    public ProjectService(IProjectRepository projectRepository, 
        ILogService logService, 
        IUserRepository userRepository, 
        IMapper mapper, 
        INotificationsService notificationsService)
    {
        _projectRepository = projectRepository;
        _logService = logService;
        _userRepository = userRepository;
        _mapper = mapper;
        _notificationsService = notificationsService;
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
            ValidateProject(projectName, projectDetails, account, ref result);

            if (result.Errors.Any())
            {
                result.IsSuccess = false;
                
                return result;
            }

            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId <= 0)
            {
                throw new NotFoundUserIdByAccountException(account);
            }
            
            // Проверяем существование такого проекта у текущего пользователя.
            var isCreatedProject = await _projectRepository.CheckCreatedProjectByProjectNameAsync(projectName, userId);
            //
            // // Есть дубликат, нельзя создать проект.
            if (isCreatedProject)
            {
                await _notificationsService.SendNotificationWarningDublicateUserProjectAsync("Увы...", "Такой проект у вас уже существует!", NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, null);
                result.IsSuccess = false;
                
                return result;
            }
            
            var statusId = ProjectStatus.GetProjectStatusIdBySysName(ProjectStatusNameEnum.Moderation.ToString());
            var statusName = ProjectStatus.GetProjectStatusNameBySysName(ProjectStatusNameEnum.Moderation.ToString());
            var project = await _projectRepository.CreateProjectAsync(projectName, projectDetails, userId, ProjectStatusNameEnum.Moderation.ToString(), statusId, statusName);
                
            // Если что то пошло не так при создании проекта.
            if (project?.ProjectId <= 0)
            {
                var ex = new Exception("Ошибка при создании проекта!");
                await _logService.LogCriticalAsync(ex);
                await _notificationsService.SendNotificationErrorCreatedUserProjectAsync("Что то пошло не так", "Ошибка при создании проекта. Мы уже знаем о проблеме и уже занимаемся ей.", NotificationLevelConsts.NOTIFICATION_LEVEL_ERROR, null);
                
                result.IsSuccess = false;
                
                return result;
            }

            result = _mapper.Map<CreateProjectOutput>(project);

            // Отправляем уведомление об успешном создании проекта.
            await _notificationsService.SendNotificationSuccessCreatedUserProjectAsync("Все хорошо", "Данные успешно сохранены! Проект отправлен на модерацию!", NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, null);
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
    /// Метод валидация проекта при его создании.
    /// </summary>
    /// <param name="projectName"></param>
    /// <param name="projectDetails"></param>
    /// <param name="account"></param>
    /// <param name="result">Результирующая модель. Тут не возвращается, так как передана по ссылке сюда.</param>
    private void ValidateProject(string projectName, string projectDetails, string account, ref CreateProjectOutput result)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            result.Errors.Add("Не заполнено название проекта!");
        }
        
        if (string.IsNullOrEmpty(projectDetails))
        {
            result.Errors.Add("Не заполнено описание проекта!");
        }
        
        if (string.IsNullOrEmpty(account))
        {
            var ex = new ArgumentNullException($"Не передан аккаунт пользователя!");
            _logService.LogError(ex);
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
                throw new NullReferenceException("Не удалось получить поля для таблицы UserProjects!");
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
}