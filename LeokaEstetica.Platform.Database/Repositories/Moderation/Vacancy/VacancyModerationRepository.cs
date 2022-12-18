using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Moderation;
using LeokaEstetica.Platform.Models.Entities.Moderation;

namespace LeokaEstetica.Platform.Database.Repositories.Moderation.Vacancy;

/// <summary>
/// Класс реализует методы репозитория модерации вакансий.
/// </summary>
public sealed class VacancyModerationRepository : IVacancyModerationRepository
{
    private readonly PgContext _pgContext;
    
    public VacancyModerationRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод отправляет вакансию на модерацию. Это происходит через добавление в таблицу модерации вакансий.
    /// Если вакансия в этой таблице, значит она не прошла еще модерацию. При прохождении модерации она удаляется из нее.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    public async Task AddVacancyModerationAsync(long vacancyId)
    {
        var vacancy = new ModerationVacancyEntity
        {
            VacancyId = vacancyId,
            DateModeration = DateTime.Now
        };
        await _pgContext.ModerationVacancies.AddAsync(vacancy);
        await _pgContext.SaveChangesAsync();
    }
}