namespace LeokaEstetica.Platform.Moderation.Abstractions.Vacancy;

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
}