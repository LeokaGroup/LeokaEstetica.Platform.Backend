using LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Vacancy;

namespace LeokaEstetica.Platform.CallCenter.Abstractions.Vacancy;

/// <summary>
/// Абстракция сервиса модерации вакансий.
/// </summary>
public interface IVacancyModerationService
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
    Task<VacanciesModerationResult> VacanciesModerationAsync();
    
    /// <summary>
    /// Метод одобряет вакансию на модерации.
    /// </summary>
    /// <param name="projectId">Id вакансии.</param>
    /// <returns>Выходная модель модерации.</returns>
    Task<ApproveVacancyOutput> ApproveVacancyAsync(long vacancyId);
    
    /// <summary>
    /// Метод отклоняет вакансию на модерации.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Выходная модель модерации.</returns>
    Task<RejectVacancyOutput> RejectVacancyAsync(long vacancyId);
}