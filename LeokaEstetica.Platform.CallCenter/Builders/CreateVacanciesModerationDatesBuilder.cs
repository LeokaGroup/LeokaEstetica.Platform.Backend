using System.Globalization;
using AutoMapper;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Moderation;

namespace LeokaEstetica.Platform.CallCenter.Builders;

/// <summary>
/// Билдер строит даты вакансий модерации к нужному виду.
/// </summary>
public static class CreateVacanciesModerationDatesBuilder
{
    private static readonly List<VacancyModerationOutput> _vacancies = new();

    /// <summary>
    /// Метод форматирует даты к нужному виду для модерации.
    /// </summary>
    /// <param name="projects">Список вакансий из БД.</param>
    /// <param name="mapper">Автомаппер.</param>
    /// <returns>Список с измененными датами.</returns>
    public static IEnumerable<VacancyModerationOutput> Create(IEnumerable<ModerationVacancyEntity> vacancies,
        IMapper mapper)
    {
        _vacancies.Clear();

        foreach (var item in vacancies)
        {
            // Прежде чем мапить форматируем даты.
            var convertModerationDate = item.DateModeration.ToString("g", CultureInfo.GetCultureInfo("ru"));
            var convertCreatedDate = item.UserVacancy.DateCreated.ToString("g", CultureInfo.GetCultureInfo("ru"));

            // Затем уже мапим к результирующей модели.
            var newItem = mapper.Map<VacancyModerationOutput>(item);
            newItem.DateModeration = convertModerationDate;
            newItem.DateCreated = convertCreatedDate;
            newItem.VacancyName = item.UserVacancy.VacancyName;
            newItem.ModerationStatusName = item.ModerationStatus.StatusName;
            newItem.IsPaymentCompleted = item.UserVacancy.IsPaymentCompleted;
            _vacancies.Add(newItem);
        }

        return _vacancies;
    }
}