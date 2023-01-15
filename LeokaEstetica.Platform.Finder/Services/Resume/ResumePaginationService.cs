using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Finder.Abstractions.Resume;
using LeokaEstetica.Platform.Finder.Builders;
using LeokaEstetica.Platform.Finder.Chains;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Finder.Loaders;
using LeokaEstetica.Platform.Logs.Abstractions;
using LeokaEstetica.Platform.Models.Dto.Output.Pagination;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace LeokaEstetica.Platform.Finder.Services.Resume;

/// <summary>
/// Класс реализует методы сервиса пагинации резюме.
/// </summary>
public class ResumePaginationService : BaseIndexRamDirectory, IResumePaginationService
{
    private readonly IResumeRepository _resumeRepository;
    private readonly ILogService _logService;

    public ResumePaginationService(IResumeRepository resumeRepository,
        ILogService logService)
    {
        _resumeRepository = resumeRepository;
        _logService = logService;
    }
    
    /// <summary>
    /// Метод пагинации резюме.
    /// </summary>
    /// <param name="page">Номер страницы.</param>
    /// <returns>Список резюме.</returns>
    public async Task<PaginationResumeOutput> GetResumesPaginationAsync(int page)
    {
        try
        {
            var projects = await _resumeRepository.GetFilterResumesAsync();
            var result = new PaginationResumeOutput
            {
                IsVisiblePagination = true,
                PaginationInfo = new PaginationInfoOutput(projects.Count(), page, PaginationConst.TAKE_COUNT)
            };

            // Получаем все проекты из БД без выгрузки в память.
            ResumesDocumentLoader.Load(projects, _index, _analyzer);

            using var reader = IndexReader.Open(_index.Value, true);
            using var searcher = new IndexSearcher(reader);
            var scoreDocs = CreateScoreDocsBuilder.CreateScoreDocsResult(page, searcher);

            result.Resumes = CreateResumesSearchResultBuilder
                .CreateResumesSearchResult(scoreDocs, searcher)
                .ToList();

            // Если первая страница и записей менее максимального на странице,
            // то надо скрыть пагинацию, так как смысл в пагинации теряется в этом кейсе.
            // Применяем именно к 1 странице, к последней нет (там это надо показывать).
            if (page == 1 && result.Resumes.Count < PaginationConst.TAKE_COUNT)
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