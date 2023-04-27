using AutoMapper;
using LeokaEstetica.Platform.Base.Extensions.HtmlExtensions;
using LeokaEstetica.Platform.CallCenter.Abstractions.Messaging.Mail;
using LeokaEstetica.Platform.CallCenter.Abstractions.Project;
using LeokaEstetica.Platform.CallCenter.Builders;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Project;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Input.Moderation;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;

namespace LeokaEstetica.Platform.CallCenter.Services.Project;

/// <summary>
/// Класс реализует методы сервиса модерации проектов.
/// </summary>
public class ProjectModerationService : IProjectModerationService
{
    private readonly IProjectModerationRepository _projectModerationRepository;
    private readonly ILogService _logService;
    private readonly IMapper _mapper;
    private readonly IModerationMailingsService _moderationMailingsService;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectModerationNotificationService _projectModerationNotificationService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectModerationRepository">Репозиторий модерации проектов.</param>
    /// <param name="logService">Сервис логов.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="moderationMailingsService">Сервис уведомлений модерации на почту.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    /// <param name="projectModerationNotificationService">Сервис уведомлений модерации проектов.</param>
    public ProjectModerationService(IProjectModerationRepository projectModerationRepository,
        ILogService logService,
        IMapper mapper, 
        IModerationMailingsService moderationMailingsService, 
        IUserRepository userRepository, 
        IProjectRepository projectRepository, 
        IProjectModerationNotificationService projectModerationNotificationService)
    {
        _projectModerationRepository = projectModerationRepository;
        _logService = logService;
        _mapper = mapper;
        _moderationMailingsService = moderationMailingsService;
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _projectModerationNotificationService = projectModerationNotificationService;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список проектов для модерации.
    /// </summary>
    /// <returns>Список проектов.</returns>
    public async Task<ProjectsModerationResult> ProjectsModerationAsync()
    {
        try
        {
            var result = new ProjectsModerationResult();
            var items = await _projectModerationRepository.ProjectsModerationAsync();
            result.Projects = CreateProjectsModerationDatesBuilder.Create(items, _mapper);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает проект для просмотра/изменения.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные проекта.</returns>
    public async Task<ProjectOutput> GetProjectModerationByProjectIdAsync(long projectId)
    {
        try
        {
            var prj = await _projectRepository.GetProjectAsync(projectId);
            var result = await CreateProjectResultAsync(prj);
            result.ProjectDetails = ClearHtmlBuilder.Clear(prj.UserProject.ProjectDetails);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex, "Ошибка при получении проекта для модерации. " +
                                                $"ProjectId = {projectId}");
            throw;
        }
    }

    /// <summary>
    /// Метод одобряет проект на модерации.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Выходная модель модерации.</returns>
    public async Task<ApproveProjectOutput> ApproveProjectAsync(long projectId, string account)
    {
        try
        {
            var result = new ApproveProjectOutput
            {
                IsSuccess = await _projectModerationRepository.ApproveProjectAsync(projectId)
            };
            
            if (!result.IsSuccess)
            {
                var ex = new InvalidOperationException($"Ошибка при одобрении проекта. ProjectId: {projectId}");
                throw ex;
            }
            
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var user = await _userRepository.GetUserPhoneEmailByUserIdAsync(userId);

            var projectName = await _projectRepository.GetProjectNameByIdAsync(projectId);
            
            // Отправляем уведомление на почту владельца проекта.
            await _moderationMailingsService.SendNotificationApproveProjectAsync(user.Email, projectName, projectId);
            
            // Отправляем уведомление в приложении об одобрении проекта модератором.
            await _projectModerationRepository.AddNotificationApproveProjectAsync(projectId, userId, projectName);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex, "Ошибка при одобрении проекта при модерации. " +
                                                $"ProjectId = {projectId}");
            throw;
        }
    }

    /// <summary>
    /// Метод отклоняет проект на модерации.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Выходная модель модерации.</returns>
    public async Task<RejectProjectOutput> RejectProjectAsync(long projectId, string account)
    {
        try
        {
            var result = new RejectProjectOutput
            {
                IsSuccess = await _projectModerationRepository.RejectProjectAsync(projectId)
            };
            
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var user = await _userRepository.GetUserPhoneEmailByUserIdAsync(userId);

            var projectName = await _projectRepository.GetProjectNameByIdAsync(projectId);
            
            // Отправляем уведомление на почту владельца проекта.
            await _moderationMailingsService.SendNotificationRejectProjectAsync(user.Email, projectName, projectId);
            
            // Отправляем уведомление в приложении об отклонении проекта модератором.
            await _projectModerationRepository.AddNotificationRejectProjectAsync(projectId, userId, projectName);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex,
                $"Ошибка при отклонении проекта при модерации. ProjectId = {projectId}");
            throw;
        }
    }

    /// <summary>
    /// Метод создает замечания проекта.
    /// </summary>
    /// <param name="createProjectRemarkInput">Список замечаний.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>
    /// <returns>Список замечаний проекта.</returns>
    public async Task<IEnumerable<ProjectRemarkEntity>> CreateProjectRemarksAsync(
        CreateProjectRemarkInput createProjectRemarkInput, string account, string token)
    {
        try
        {
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var projectRemarks = createProjectRemarkInput.ProjectRemarks.ToList();
            
            // Проверяем входные параметры.
            await ValidateProjectRemarksParamsAsync(projectRemarks);

            var mapProjectRemarks = _mapper.Map<List<ProjectRemarkEntity>>(projectRemarks);

            // Получаем названия полей.
            var fields = projectRemarks.Select(pr => pr.FieldName);
            
            // Оставляем лишь те замечания, которые не были добавлены к проекту.
            // Проверяем по названию замечания и по статусу.
            // Тут First можем, так как валидацию на ProjectId проводили выше.
            var projectId = createProjectRemarkInput.ProjectRemarks.First().ProjectId;
            
            // Получаем замечания, которые модератор уже сохранял в рамках текущего проекта.
            var existsProjectRemarks = await _projectModerationRepository.GetExistsProjectRemarksAsync(projectId,
                fields);
            
            var now = DateTime.Now;
            var addProjectRemarks = new List<ProjectRemarkEntity>();
            var updateProjectRemarks = new List<ProjectRemarkEntity>();
            
            // Задаем модератора замечаниям и задаем статус замечаниям.
            foreach (var pr in mapProjectRemarks)
            {
                pr.ModerationUserId = userId;
                pr.RemarkStatusId = (int)RemarkStatusEnum.NotAssigned;
                pr.DateCreated = now;
                
                // Если есть замечания проекта сохраненные ранее.
                if (existsProjectRemarks.Any())
                {
                    var getProjectRemarks = existsProjectRemarks.Find(x => x.FieldName.Equals(pr.FieldName));
                    
                    // К обновлению.
                    if (getProjectRemarks is not null)
                    {
                        pr.RemarkId = getProjectRemarks.RemarkId;
                        updateProjectRemarks.Add(pr);   
                    }
                    
                    // К добавлению.
                    else
                    {
                        addProjectRemarks.Add(pr);
                    }
                }

                else
                {
                    addProjectRemarks.Add(pr);
                }
            }

            // Добавляем новые замечания проекта.
            if (addProjectRemarks.Any())
            {
                await _projectModerationRepository.CreateProjectRemarksAsync(addProjectRemarks);   
            }
            
            // Изменяем замечания проекта, которые ранее были сохранены.
            if (updateProjectRemarks.Any())
            {
                await _projectModerationRepository.UpdateProjectRemarksAsync(updateProjectRemarks);   
            }

            var result = addProjectRemarks.Union(updateProjectRemarks);

            if (!string.IsNullOrEmpty(token))
            {
                // Отправляем уведомление о сохранении замечаний проекта.
                await _projectModerationNotificationService.SendNotificationSuccessCreateProjectRemarksAsync(
                    "Все хорошо", "Замечания успешно внесены. Теперь вы можете их отправить.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
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
    /// Метод отправляет замечания проекта владельцу проекта.
    /// Отправка замечаний проекту подразумевает просто изменение статуса замечаниям проекта.
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>
    /// </summary>
    public async Task SendProjectRemarksAsync(long projectId, string account, string token)
    {
        try
        {
            var userId = await _userRepository.GetUserIdByEmailOrLoginAsync(account);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            if (projectId <= 0)
            {
                var ex = new InvalidOperationException($"Id проекта не был передан. ProjectId: {projectId}");
                throw ex;
            }
            
            // Проверяем, были ли внесены замечания проекта.
            var isExists = await _projectModerationRepository.CheckExistsProjectRemarksAsync(projectId, userId);

            if (!isExists && !string.IsNullOrEmpty(token))
            {
                // Отправляем уведомление о предупреждении, что замечания проекта не были внесены.
                await _projectModerationNotificationService.SendNotificationWarningSendProjectRemarksAsync(
                    "Внимание", "Замечания не были внесены. Для отправки замечаний они должны быть внесены.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
            }
            
            await _projectModerationRepository.SendProjectRemarksAsync(projectId, userId);   
            
            if (!string.IsNullOrEmpty(token))
            {
                // Отправляем уведомление об отправке замечаний проекта.
                await _projectModerationNotificationService.SendNotificationSuccessSendProjectRemarksAsync(
                    "Все хорошо", "Замечания успешно отправлены.",
                    NotificationLevelConsts.NOTIFICATION_LEVEL_SUCCESS, token);
            }
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
    /// Метод создает результаты проекта. 
    /// </summary>
    /// <param name="prj">Данные проекта.</param>
    /// <returns>Результаты проекта.</returns>
    private async Task<ProjectOutput> CreateProjectResultAsync(
        (UserProjectEntity UserProject, ProjectStageEntity ProjectStage) prj)
    {
        var result = _mapper.Map<ProjectOutput>(prj.UserProject);
        result.StageId = prj.ProjectStage.StageId;
        result.StageName = prj.ProjectStage.StageName;
        result.StageSysName = prj.ProjectStage.StageSysName;
        result.IsSuccess = true;

        return await Task.FromResult(result);
    }

    /// <summary>
    /// Метод валидирует входные параметры замечаний проекта.
    /// </summary>
    /// <param name="projectRemarks">Список замечаний проекта.</param>
    private Task ValidateProjectRemarksParamsAsync(IReadOnlyCollection<ProjectRemarkInput> projectRemarks)
    {
        if (!projectRemarks.Any())
        {
            var ex = new InvalidOperationException("Не передано замечаний проекта.");
            throw ex;
        }
        
        if (projectRemarks.Any(p => p.ProjectId <= 0))
        {
            var ex = new InvalidOperationException("Среди замечаний проекта Id проекта был <= 0.");
            throw ex;
        }

        if (projectRemarks.Any(p => string.IsNullOrEmpty(p.FieldName)))
        {
            var ex = new InvalidOperationException("Среди замечаний проекта было пустое название поля замечания.");
            throw ex;
        }

        if (projectRemarks.Any(p => string.IsNullOrEmpty(p.RemarkText)))
        {
            var ex = new InvalidOperationException("Среди замечаний проекта был пустой текст замечания.");
            throw ex;
        }
        
        if (projectRemarks.Any(p => string.IsNullOrEmpty(p.RemarkText)))
        {
            var ex = new InvalidOperationException("Среди замечаний проекта был пустой текст замечания.");
            throw ex;
        }
        
        if (projectRemarks.Any(p => string.IsNullOrEmpty(p.RussianName)))
        {
            var ex = new InvalidOperationException("Среди замечаний проекта было пустое русское название поля.");
            throw ex;
        }

        return Task.CompletedTask;
    }

    #endregion
}