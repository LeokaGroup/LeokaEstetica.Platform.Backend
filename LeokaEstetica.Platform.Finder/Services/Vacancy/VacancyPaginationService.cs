using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Finder.Abstractions.Vacancy;
using LeokaEstetica.Platform.Finder.Builders;
using LeokaEstetica.Platform.Finder.Chains;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Finder.Loaders;
using LeokaEstetica.Platform.Models.Dto.Output.Pagination;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Finder.Services.Vacancy;

/// <summary>
/// Класс реализует методы сервиса пагинации вакансий.
/// </summary>
public class VacancyPaginationService : BaseIndexRamDirectory, IVacancyPaginationService
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly ILogger<VacancyPaginationService> _logger;

    public VacancyPaginationService(IVacancyRepository vacancyRepository,
        ILogger<VacancyPaginationService> logger)
    {
        _vacancyRepository = vacancyRepository;
        _logger = logger;
    }

    /// <summary>
    /// Метод пагинации вакансий.
    /// </summary>
    /// <param name="page">Номер страницы.</param>
    /// <returns>Список вакансий.</returns>
    public async Task<PaginationVacancyOutput> GetVacanciesPaginationAsync(int page)
    {
        try
        {
            var vacancies = await _vacancyRepository.GetFiltersVacanciesAsync();
            var result = new PaginationVacancyOutput
            {
                IsVisiblePagination = true,
                PaginationInfo = new PaginationInfoOutput(vacancies.Count(), page, PaginationConst.TAKE_COUNT)
            };

            // Получаем все вакансии из БД без выгрузки в память.
            VacanciesDocumentLoader.Load(vacancies, _index, _analyzer);

            using var reader = IndexReader.Open(_index.Value, true);
            using var searcher = new IndexSearcher(reader);
            var scoreDocs = CreateScoreDocsBuilder.CreateScoreDocsResult(page, searcher, vacancies.Count());

            result.Vacancies = CreateVacanciesSearchResultBuilder
                .CreateVacanciesSearchResult(scoreDocs, searcher)
                .ToList();

            // Если первая страница и записей менее максимального на странице,
            // то надо скрыть пагинацию, так как смысл в пагинации теряется в этом кейсе.
            // Применяем именно к 1 странице, к последней нет (там это надо показывать).
            if (page == 1 && result.Vacancies.Count < PaginationConst.TAKE_COUNT)
            {
                result.IsVisiblePagination = false;
            }

            return result;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}