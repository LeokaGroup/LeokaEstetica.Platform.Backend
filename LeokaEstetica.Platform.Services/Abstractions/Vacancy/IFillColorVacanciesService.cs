using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

namespace LeokaEstetica.Platform.Services.Abstractions.Vacancy;

public interface IFillColorVacanciesService
{
    public Task SetColorBusinessVacanciesAsync(IEnumerable<CatalogVacancyOutput> vacancies);
}
