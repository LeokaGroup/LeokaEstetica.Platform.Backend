using AutoMapper;
using FluentValidation.Results;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Base.Extensions.HtmlExtensions;
using LeokaEstetica.Platform.CallCenter.Abstractions.Messaging.Mail;
using LeokaEstetica.Platform.CallCenter.Abstractions.Project;
using LeokaEstetica.Platform.CallCenter.Builders;
using LeokaEstetica.Platform.CallCenter.Consts;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Project;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Integrations.Abstractions.Pachca;
using LeokaEstetica.Platform.Integrations.Enums;
using LeokaEstetica.Platform.Models.Dto.Input.Moderation;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Notifications.Abstractions;
using LeokaEstetica.Platform.Notifications.Consts;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.CallCenter.Services.Project;

/// <summary>
/// Класс реализует методы сервиса модерации проектов.
/// </summary>
internal sealed class ProjectModerationService : IProjectModerationService
{
    private readonly IProjectModerationRepository _projectModerationRepository;
    private readonly ILogger<ProjectModerationService> _logger;
    private readonly IMapper _mapper;
    private readonly IModerationMailingsService _moderationMailingsService;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectModerationNotificationService _projectModerationNotificationService;
    private readonly IPachcaService _pachcaService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectModerationRepository">Репозиторий модерации проектов.</param>
    /// <param name="logger">Сервис логов.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="moderationMailingsService">Сервис уведомлений модерации на почту.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    /// <param name="projectModerationNotificationService">Сервис уведомлений модерации проектов.</param>
    /// <param name="pachcaService">Сервис пачки.</param>
    public ProjectModerationService(IProjectModerationRepository projectModerationRepository,
        ILogger<ProjectModerationService> logger,
        IMapper mapper, 
        IModerationMailingsService moderationMailingsService, 
        IUserRepository userRepository, 
        IProjectRepository projectRepository, 
        IProjectModerationNotificationService projectModerationNotificationService,
        IPachcaService pachcaService)
    {
        _projectModerationRepository = projectModerationRepository;
        _logger = logger;
        _mapper = mapper;
        _moderationMailingsService = moderationMailingsService;
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _projectModerationNotificationService = projectModerationNotificationService;
        _pachcaService = pachcaService;
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
            _logger.LogError(ex, ex.Message);
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
            _logger.LogError(ex, "Ошибка при получении проекта для модерации. " +
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

            await _pachcaService.SendNotificationCreatedObjectAsync(ObjectTypeEnum.Project, projectName);

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при одобрении проекта при модерации. " +
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
            // TODO: Надо еще проверять, что внесены замечания проекта. Нельзя отклонить проект, не внеся замечания,
            // TODO: и модератор должен это видеть.
            // TODO: Добавить такую проверку тут.
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
            _logger.LogError(ex, $"Ошибка при отклонении проекта при модерации. ProjectId = {projectId}");
            throw;
        }
    }

    /// <summary>
    /// Метод создает замечания проекта. 
    /// </summary>
    /// <param name="createProjectRemarkInput">Входная модель.</param>
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

            // Оставляем лишь те замечания, которые не были добавлены к проекту.
            // Проверяем по названию замечания и по статусу.
            var projectId = projectRemarks.FirstOrDefault()?.ProjectId;

            if (projectId is null or <= 0)
            {
                var ex = new InvalidOperationException($"Id проекта не был передан. ProjectId: {projectId}");
                throw ex;
            }

            var now = DateTime.UtcNow;
            var addProjectRemarks = new List<ProjectRemarkEntity>();
            var updateProjectRemarks = new List<ProjectRemarkEntity>();
            
            var mapProjectRemarks = _mapper.Map<List<ProjectRemarkEntity>>(projectRemarks);
            
            // Получаем названия полей.
            var fields = projectRemarks.Select(pr => pr.FieldName);
            
            // Получаем замечания, которые модератор уже сохранял в рамках текущего проекта.
            var existsProjectRemarks = await _projectModerationRepository.GetExistsProjectRemarksAsync((long)projectId,
                fields);
            
            // Задаем модератора замечаниям и задаем статус замечаниям.
            foreach (var pr in mapProjectRemarks)
            {
                pr.ModerationUserId = userId;
                pr.RemarkStatusId = (int)RemarkStatusEnum.NotAssigned;
                pr.DateCreated = now;
                
                // Если есть замечания проекта сохраненные ранее.
                if (existsProjectRemarks.Any())
                {
                    var getProjectRemarks = existsProjectRemarks.Find(x => x.FieldName.Equals(pr.FieldName)
                                                                           && x.ProjectId == pr.ProjectId);
                    
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
            _logger.LogError(ex, ex.Message);
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
            if (projectId <= 0)
            {
                var ex = new InvalidOperationException($"Id проекта не был передан. ProjectId: {projectId}");
                throw ex;
            }
            
            // Проверяем, были ли внесены замечания проекта.
            var isExists = await _projectModerationRepository.CheckExistsProjectRemarksAsync(projectId);

            if (!isExists)
            {
                var ex = new InvalidOperationException(RemarkConst.SEND_PROJECT_REMARKS_WARNING +
                                                       $" ProjectId: {projectId}");
                _logger.LogWarning(ex, ex.Message);

                if (!string.IsNullOrEmpty(token))
                {
                    // Отправляем уведомление о предупреждении, что замечания проекта не были внесены.
                    await _projectModerationNotificationService.SendNotificationWarningSendProjectRemarksAsync(
                        "Внимание", RemarkConst.SEND_PROJECT_REMARKS_WARNING,
                        NotificationLevelConsts.NOTIFICATION_LEVEL_WARNING, token);
                }

                return;
            }
            
            await _projectModerationRepository.SendProjectRemarksAsync(projectId);

            var projectName = await _projectModerationRepository.GetProjectNameAsync(projectId);
            var remarks = await _projectModerationRepository.GetProjectRemarksAsync(projectId);
            await _moderationMailingsService.SendNotificationAboutRemarkAsync(account, projectName, remarks);
            
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
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список замечаний проекта (не отправленные), если они есть.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список замечаний проекта.</returns>
    public async Task<IEnumerable<ProjectRemarkEntity>> GetProjectUnShippedRemarksAsync(long projectId)
    {
        if (projectId <= 0)
        {
            var ex = new InvalidOperationException($"Id проекта не был передан. ProjectId: {projectId}");
            throw ex;
        }
        
        var result = await _projectModerationRepository.GetProjectUnShippedRemarksAsync(projectId);

        return result;
    }

    /// <summary>
    /// Метод получает комментарий проекта для просмотра.
    /// </summary>
    /// <param name="commentId">Id комментария.</param>
    /// <returns>Данные комментария.</returns>
    public async Task<ProjectCommentModerationEntity> GetCommentModerationByCommentIdAsync(long commentId)
    {
        try
        {
            var result = await _projectModerationRepository.GetCommentModerationByCommentIdAsync(commentId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод одобряет комментарий проекта.
    /// </summary>
    /// <param name="commentId">Id комментарии.</param>
    /// <returns>Признак успешного подверждения.</returns>
    public async Task<bool> ApproveProjectCommentAsync(long commentId)
    {
        try
        {
            var result = await _projectModerationRepository.ApproveProjectCommentAsync(commentId);

            if (!result)
            {
                throw new InvalidOperationException("Ошибка при подтверждении комментария проекта." +
                                                    $" CommentId: {commentId}");
            }

            return true;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод отклоняет комментарий проекта.
    /// </summary>
    /// <param name="commentId">Id комментарии.</param>
    /// <returns>Признак успешного подверждения.</returns>
    public async Task<ManagingProjectCommentModerationOutput> RejectProjectCommentAsync(long commentId)
    {
        try
        {
            var result = new ManagingProjectCommentModerationOutput
            {
                Errors = new List<ValidationFailure>()
            };
            
            // Проверяем, внесены ли замечания к комментарию. Если нет, то не даем отклонить без указания замечаний.
            var isWriteRemarks = await _projectModerationRepository.IfRemarksProjectCommentAsync(commentId);

            // Если нет зафиксированных замечаний комментария, то не даем отклонить комментарий проекта.
            if (!isWriteRemarks)
            {
                result.Errors.Add(new ValidationFailure
                {
                    ErrorMessage = "Нельзя отклонить комментарий проекта." +
                                   " Сначала нужно внести замечания и затем их отправить пользователю."
                });

                _logger.LogInformation("Не удалось отклонить комментарий проекта." +
                                       " Не были внесены замечания." +
                                       $" CommentId: {commentId}");
                
                return result;
            }

            var isReject = await _projectModerationRepository.RejectProjectCommentAsync(commentId);

            if (!isReject)
            {
                var ex = new InvalidOperationException(
                    $"Ошибка при отклонении комментария проекта. CommentId: {commentId}");
                throw ex;
            }
            
            result.IsSuccess = true;
            
            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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