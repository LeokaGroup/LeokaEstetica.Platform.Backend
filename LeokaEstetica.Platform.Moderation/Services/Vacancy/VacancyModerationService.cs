using LeokaEstetica.Platform.Database.Abstractions.Moderation.Vacancy;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using LeokaEstetica.Platform.Moderation.Abstractions.Vacancy;

namespace LeokaEstetica.Platform.Moderation.Services.Vacancy;

/// <summary>
/// Класс реализует методы сервиса модерации вакансий.
/// </summary>
public sealed class VacancyModerationService : IVacancyModerationService
{
    private readonly IVacancyModerationRepository _vacancyModerationRepository;
    private readonly ILogService _logService;

    public VacancyModerationService(IVacancyModerationRepository vacancyModerationRepository,
        ILogService logService)
    {
        _vacancyModerationRepository = vacancyModerationRepository;
        _logService = logService;
    }

    /// <summary>
    /// Метод отправляет вакансию на модерацию. Это происходит через добавление в таблицу модерации вакансий.
    /// Если вакансия в этой таблице, значит она не прошла еще модерацию. При прохождении модерации она удаляется из нее.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    public async Task AddVacancyModerationAsync(long vacancyId)
    {
        try
        {
            await _vacancyModerationRepository.AddVacancyModerationAsync(vacancyId);
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }

    /// <summary>
    /// Метод получает вакансию для просмотра.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Данные вакансии.</returns>
    public async Task<UserVacancyEntity> GetVacancyModerationByVacancyIdAsync(long vacancyId)
    {
        try
        {
            var result = await _vacancyModerationRepository.GetVacancyModerationByVacancyIdAsync(vacancyId);

            return result;
        }

        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex,
                $"Ошибка при получении вакансии для модерации. VacancyId = {vacancyId}");
            throw;
        }
    }
}