using AutoMapper;
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
using LeokaEstetica.Platform.Models.Entities.Project;

namespace LeokaEstetica.Platform.CallCenter.Services.Project;

/// <summary>
/// Класс реализует методы сервиса модерации проектов.
/// </summary>
public sealed class ProjectModerationService : IProjectModerationService
{
    private readonly IProjectModerationRepository _projectModerationRepository;
    private readonly ILogService _logService;
    private readonly IMapper _mapper;
    private readonly IModerationMailingsService _moderationMailingsService;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;

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
    public async Task<UserProjectEntity> GetProjectModerationByProjectIdAsync(long projectId)
    {
        try
        {
            var result = await _projectModerationRepository.GetProjectModerationByProjectIdAsync(projectId);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex, $"Ошибка при получении проекта для модерации. ProjectId = {projectId}");
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
}