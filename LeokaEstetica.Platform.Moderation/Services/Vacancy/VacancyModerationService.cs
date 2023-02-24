using AutoMapper;
using LeokaEstetica.Platform.Database.Abstractions.Moderation.Vacancy;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using LeokaEstetica.Platform.Moderation.Abstractions.Vacancy;
using LeokaEstetica.Platform.Moderation.Builders;
using LeokaEstetica.Platform.Moderation.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Moderation.Models.Dto.Output.Vacancy;

namespace LeokaEstetica.Platform.Moderation.Services.Vacancy;

/// <summary>
/// Класс реализует методы сервиса модерации вакансий.
/// </summary>
public sealed class VacancyModerationService : IVacancyModerationService
{
    private readonly IVacancyModerationRepository _vacancyModerationRepository;
    private readonly ILogService _logService;
    private readonly IMapper _mapper;

    public VacancyModerationService(IVacancyModerationRepository vacancyModerationRepository,
        ILogService logService, 
        IMapper mapper)
    {
        _vacancyModerationRepository = vacancyModerationRepository;
        _logService = logService;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод отправляет вакансию на модерацию. Это происходит через добавление в таблицу модерации вакансий.
    /// Если вакансия в этой таблице, значит она не прошла еще модерацию. При прохождении модерации она удаляется из нее.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    public async Task AddVacancyModerationAsync(long vacancyId)
    {
        await _vacancyModerationRepository.AddVacancyModerationAsync(vacancyId);
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

    /// <summary>
    /// Метод получает список вакансий для модерации.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    public async Task<VacanciesModerationResult> VacanciesModerationAsync()
    {
        try
        {
            var result = new VacanciesModerationResult();
            var items = await _vacancyModerationRepository.VacanciesModerationAsync();
            result.Vacancies = CreateVacanciesModerationDatesBuilder.Create(items, _mapper);

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
    
    /// <summary>
    /// Метод одобряет вакансию на модерации.
    /// </summary>
    /// <param name="projectId">Id вакансии.</param>
    /// <returns>Выходная модель модерации.</returns>
    public async Task<ApproveVacancyOutput> ApproveVacancyAsync(long vacancyId)
    {
        try
        {
            var result = new ApproveVacancyOutput
            {
                IsSuccess = await _vacancyModerationRepository.ApproveVacancyAsync(vacancyId)
            };

            if (!result.IsSuccess)
            {
                var ex = new InvalidOperationException($"Ошибка при одобрении вакансии. VacancyId: {vacancyId}");
                throw ex;
            }

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex,
                $"Ошибка при одобрении вакансии при модерации. VacancyId = {vacancyId}");
            throw;
        }
    }

    /// <summary>
    /// Метод отклоняет вакансию на модерации.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Выходная модель модерации.</returns>
    public async Task<RejectVacancyOutput> RejectVacancyAsync(long vacancyId)
    {
        try
        {
            var result = new RejectVacancyOutput
            {
                IsSuccess = await _vacancyModerationRepository.RejectVacancyAsync(vacancyId)
            };

            return result;
        }
        
        catch (Exception ex)
        {
            await _logService.LogErrorAsync(ex,
                $"Ошибка при отклонении вакансии при модерации. VacancyId = {vacancyId}");
            throw;
        }
    }
}