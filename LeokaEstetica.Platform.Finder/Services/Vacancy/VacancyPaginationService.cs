using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Finder.Abstractions.Vacancy;
using LeokaEstetica.Platform.Finder.Builders;
using LeokaEstetica.Platform.Finder.Chains;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Finder.Loaders;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace LeokaEstetica.Platform.Finder.Services.Vacancy;

/// <summary>
/// Класс реализует методы сервиса пагинации вакансий.
/// </summary>
public class VacancyPaginationService : BaseIndexRamDirectory, IVacancyPaginationService
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly ILogService _logService;
    private static readonly List<ScoreDoc> _scoreDocs = new();

    public VacancyPaginationService(IVacancyRepository vacancyRepository,
        ILogService logService)
    {
        _vacancyRepository = vacancyRepository;
        _logService = logService;
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
            var result = new PaginationVacancyOutput { IsVisiblePagination = true };
            var vacancies = await _vacancyRepository.GetFiltersVacanciesAsync();

            // Получаем все вакансии из БД без выгрузки в память.
            VacanciesDocumentLoader.Load(vacancies, _index, _analyzer);

            using var reader = IndexReader.Open(_index.Value, true);
            using var searcher = new IndexSearcher(reader);
            var skipRows = (page - 1) * PaginationConst.TAKE_COUNT;
            var searchResults = searcher.Search(new MatchAllDocsQuery(), null, skipRows + PaginationConst.TAKE_COUNT)
                .ScoreDocs;

            for (var i = skipRows; i < searchResults.Length; i++)
            {
                if (i > (skipRows + PaginationConst.TAKE_COUNT) - 1)
                {
                    break;
                }

                _scoreDocs.Add(searchResults[i]);
            }

            result.Vacancies = CreateVacanciesSearchResultBuilder
                .CreateVacanciesSearchResult(_scoreDocs.ToArray(), searcher)
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
            await _logService.LogErrorAsync(ex);
            throw;
        }
    }
}