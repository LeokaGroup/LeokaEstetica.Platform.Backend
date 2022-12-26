using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Messaging.Abstractions.Project;
using LeokaEstetica.Platform.Models.Entities.Communication;

namespace LeokaEstetica.Platform.Messaging.Services.Project;

/// <summary>
/// Класс реализует методы сервиса комментариев к проектам.
/// </summary>
public sealed class ProjectCommentsService : IProjectCommentsService
{
    private readonly ILogService _logService;
    private readonly IUserRepository _userRepository;
    private readonly IProjectCommentsRepository _projectCommentsRepository;

    public ProjectCommentsService(ILogService logService, 
        IUserRepository userRepository, 
        IProjectCommentsRepository projectCommentsRepository)
    {
        _logService = logService;
        _userRepository = userRepository;
        _projectCommentsRepository = projectCommentsRepository;
    }

    /// <summary>
    /// Метод создает комментарий к проекту.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="comment">Текст комментария.</param>
    /// <param name="account">Аккаунт.</param>
    public async Task CreateProjectCommentAsync(long projectId, string comment, string account)
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
            
            await _projectCommentsRepository.CreateProjectCommentAsync(projectId, comment, userId);
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex,
                "Ошибка при создании комментария к проекту. " +
                $"ProjectId = {projectId}. " +
                $"Comment = {comment}. Account = {account}");
            throw;
        }
    }

    /// <summary>
    /// Метод получает список комментариев проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список комментариев проекта.</returns>
    public async Task<IEnumerable<ProjectCommentEntity>> GetProjectCommentsAsync(long projectId)
    {
        try
        {
            var result = await _projectCommentsRepository.GetProjectCommentsAsync(projectId);

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}