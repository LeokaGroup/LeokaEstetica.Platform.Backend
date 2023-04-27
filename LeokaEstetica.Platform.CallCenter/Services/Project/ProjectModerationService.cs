using AutoMapper;
using LeokaEstetica.Platform.Base.Extensions.HtmlExtensions;
using LeokaEstetica.Platform.CallCenter.Abstractions.Messaging.Mail;
using LeokaEstetica.Platform.CallCenter.Abstractions.Project;
using LeokaEstetica.Platform.CallCenter.Builders;
using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Project;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Entities.Project;

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

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="projectModerationRepository">Репозиторий модерации проектов.</param>
    /// <param name="logService">Сервис логов.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <param name="moderationMailingsService">Сервис уведомлений модерации на почту.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="projectRepository">Репозиторий проектов.</param>
    public ProjectModerationService(IProjectModerationRepository projectModerationRepository,
        ILogService logService,
        IMapper mapper, 
        IModerationMailingsService moderationMailingsService, 
        IUserRepository userRepository, 
        IProjectRepository projectRepository)
    {
        _projectModerationRepository = projectModerationRepository;
        _logService = logService;
        _mapper = mapper;
        _moderationMailingsService = moderationMailingsService;
        _userRepository = userRepository;
        _projectRepository = projectRepository;
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

    #endregion
}