using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Vacancy;

namespace LeokaEstetica.Platform.Database.Abstractions.Moderation.Vacancy;

/// <summary>
/// Абстракция репозитория модерации вакансий.
/// </summary>
public interface IVacancyModerationRepository
{
    /// <summary>
    /// Метод отправляет вакансию на модерацию. Это происходит через добавление в таблицу модерации вакансий.
    /// Если вакансия в этой таблице, значит она не прошла еще модерацию. При прохождении модерации она удаляется из нее.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    Task AddVacancyModerationAsync(long vacancyId);
    
    /// <summary>
    /// Метод получает вакансию для просмотра.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Данные вакансии.</returns>
    Task<UserVacancyEntity> GetVacancyModerationByVacancyIdAsync(long vacancyId);
    
    /// <summary>
    /// Метод получает список вакансий для модерации.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    Task<IEnumerable<ModerationVacancyEntity>> VacanciesModerationAsync();
    
    /// <summary>
    /// Метод одобряет вакансию на модерации.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Признак подтверждения вакансии.</returns>
    Task<bool> ApproveVacancyAsync(long vacancyId);
    
    /// <summary>
    /// Метод отклоняет вакансию на модерации.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Признак подтверждения вакансии.</returns>
    Task<bool> RejectVacancyAsync(long vacancyId);

    /// <summary>
    /// Метод отправляет уведомление в приложении при одобрении вакансии модератором.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя, которому отправим уведомление в приложении.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="projectId">Id проекта.</param>
    Task AddNotificationApproveVacancyAsync(long vacancyId, long userId, string vacancyName, long projectId);
    
    /// <summary>
    /// Метод отправляет уведомление в приложении при отклонении вакансии модератором.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя, которому отправим уведомление в приложении.</param>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="projectId">Id проекта.</param>
    Task AddNotificationRejectVacancyAsync(long vacancyId, long userId, string vacancyName, long projectId);
    
    /// <summary>
    /// Метод получает замечания вакансии, которые ранее были сохранены модератором.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="fields">Список названий полей..</param>
    /// <returns>Список замечаний.</returns>
    Task<List<VacancyRemarkEntity>> GetExistsVacancyRemarksAsync(long vacancyId, IEnumerable<string> fields);
    
    /// <summary>
    /// Метод создает замечания вакансии.
    /// </summary>
    /// <param name="createVacancyRemarkInput">Список замечаний.</param>
    Task CreateVacancyRemarksAsync(IEnumerable<VacancyRemarkEntity> vacancyRemarks);
    
    /// <summary>
    /// Метод обновляет замечания вакансии.
    /// </summary>
    /// <param name="vacancyRemarks">Список замечаний для обновления.</param>
    Task UpdateVacancyRemarksAsync(List<VacancyRemarkEntity> vacancyRemarks);
    
    /// <summary>
    /// Метод отправляет замечания вакансии владельцу вакансии.
    /// Отправка замечаний вакансии подразумевает просто изменение статуса замечаниям вакансии.
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// </summary>
    Task SendVacancyRemarksAsync(long vacancyId, long userId);

    /// <summary>
    /// Метод проверяет, были ли сохранены замечания вакансии.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Признак раннего сохранения замечаний.</returns>
    Task<bool> CheckVacancyRemarksAsync(long vacancyId);

    /// <summary>
    /// Метод получает замечания вакансии.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Список замечаний.</returns>
    Task<List<VacancyRemarkEntity>> GetVacancyRemarksAsync(long vacancyId);
}