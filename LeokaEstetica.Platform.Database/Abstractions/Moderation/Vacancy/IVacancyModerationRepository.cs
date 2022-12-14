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
}