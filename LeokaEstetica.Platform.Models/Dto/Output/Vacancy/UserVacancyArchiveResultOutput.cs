namespace LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

/// <summary>
/// Класс выходной модели результата архива вакансий.
/// </summary>
public class UserVacancyArchiveResultOutput
{
    /// <summary>
    /// Список вакансий.
    /// </summary>
    public IEnumerable<VacancyArchiveOutput> VacanciesArchive { get; set; }

    /// <summary>
    /// Кол-во вакансий.
    /// </summary>
    public int Total => VacanciesArchive.Count();
}