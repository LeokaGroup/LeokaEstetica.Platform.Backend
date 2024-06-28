using LeokaEstetica.Platform.Models.Dto.Output.Notification;
using LeokaEstetica.Platform.Models.Entities.Notification;

namespace LeokaEstetica.Platform.Database.Abstractions.Notification;

/// <summary>
/// Абстракция репозитория уведомлений проектов.
/// </summary>
public interface IProjectNotificationsRepository
{
    /// <summary>
    /// Метод записывает уведомление о приглашении пользователя в проект.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectName">Название проекта.</param>
    Task AddNotificationInviteProjectAsync(long projectId, long? vacancyId, long userId, string projectName);

    /// <summary>
    /// Метод получает список уведомлений в проекты пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список уведомлений.</returns>
    Task<(List<NotificationEntity> UserNotifications, List<NotificationEntity> OwnerNotifications)>
        GetUserProjectsNotificationsAsync(long userId);
    
    /// <summary>
    /// Метод проверяет существование уведомления по его Id.
    /// </summary>
    /// <param name="notificationId">Id уведомления.</param>
    /// <returns>Признак существования уведомления.</returns>
    Task<bool> CheckExistsNotificationByIdAsync(long notificationId);

    /// <summary>
    /// Метод апрувит приглашение в проект.
    /// </summary>
    /// <param name="notificationId">Id уведомления.</param>
    Task ApproveProjectInviteAsync(long notificationId);
    
    /// <summary>
    /// Метод реджектит приглашение в проект.
    /// </summary>
    /// <param name="notificationId">Id уведомления.</param>
    Task RejectProjectInviteAsync(long notificationId);
    
    /// <summary>
    /// Метод записывает уведомление о принятии приглашения пользователя в проект.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="isProjectOwner">Признак владельца проекта.</param>
    Task AddNotificationApproveInviteProjectAsync(long projectId, long? vacancyId, long userId, string projectName,
        bool isProjectOwner);
    
    /// <summary>
    /// Метод записывает уведомление о отклонении приглашения пользователя в проект.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="isProjectOwner">Признак владельца проекта.</param>
    Task AddNotificationRejectInviteProjectAsync(long projectId, long? vacancyId, long userId, string projectName,
        bool isProjectOwner);

    /// <summary>
    /// Метод получает Id проекта по Id уведомления.
    /// </summary>
    /// <param name="notificationId">Id уведомления.</param>
    /// <returns>Id проекта.</returns>
    Task<long> GetProjectIdByNotificationIdAsync(long notificationId);

    /// <summary>
    /// Метод получает Id вакансии по Id уведомления.
    /// </summary>
    /// <param name="notificationId">Id уведомления.</param>
    /// <returns>Id вакансии.</returns>
    Task<long?> GetVacancyIdByNotificationIdAsync(long notificationId);
    
    /// <summary>
    /// Метод записывает уведомление о исключении пользователя из команды проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectName">Название проекта.</param>
    Task AddNotificationDeleteProjectTeamMemberAsync(long projectId, long? vacancyId, long userId, string projectName);
    
    /// <summary>
    /// Метод записывает уведомление о исключении пользователя из команды проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectName">Название проекта.</param>
    Task AddNotificationLeaveProjectTeamMemberAsync(long projectId, long? vacancyId, long userId, string projectName);

    /// <summary>
    /// Метод получает приглашения в проект на основе данных уведомления о приглашении.
    /// </summary>
    /// <param name="notificationId">Id уведомления.</param>
    /// <returns>Данные приглашения.</returns>
    Task<ProjectInviteNotificationOutput?> GetProjectInviteNotificationAsync(long notificationId);
}