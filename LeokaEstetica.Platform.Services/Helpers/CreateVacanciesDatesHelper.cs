using System.Globalization;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Vacancy;

namespace LeokaEstetica.Platform.Services.Helpers;

/// <summary>
/// Класс хелпера дат вакансий.
/// </summary>
public static class CreateVacanciesDatesHelper
{
    /// <summary>
    /// Метод форматирует даты вакансий.
    /// </summary>
    /// <param name="vacancies">Список вакансий из БД.</param>
    /// <param name="vacanciesArchive">Результирующий список вакансий.</param>
    public static async Task CreateDatesResultAsync(IEnumerable<ArchivedVacancyEntity> vacancies,
        List<VacancyArchiveOutput> vacanciesArchive)
    {
        var i = 0;
        
        foreach (var prj in vacancies)
        {
            vacanciesArchive[i].DateArchived = prj.DateArchived.ToString("g", CultureInfo.GetCultureInfo("ru"));
            i++;
        }

        await Task.CompletedTask;
    }
}