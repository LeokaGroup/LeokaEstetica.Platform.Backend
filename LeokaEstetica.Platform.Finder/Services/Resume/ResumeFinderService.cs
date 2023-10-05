using LeokaEstetica.Platform.Database.Abstractions.Resume;
using LeokaEstetica.Platform.Finder.Abstractions.Resume;
using LeokaEstetica.Platform.Finder.Builders;
using LeokaEstetica.Platform.Finder.Chains;
using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Finder.Loaders;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Finder.Services.Resume;

/// <summary>
/// Класс реализует методы поискового сервиса резюме.
/// </summary>
public class ResumeFinderService : BaseIndexRamDirectory, IResumeFinderService
{
    private readonly ILogger<ResumeFinderService> _logger;
    private readonly IResumeRepository _resumeRepository;
    
    public ResumeFinderService(ILogger<ResumeFinderService> logger, 
        IResumeRepository resumeRepository)
    {
        _logger = logger;
        _resumeRepository = resumeRepository;
    }

    /// <summary>
    /// Метод находит резюме по поисковому запросу.
    /// </summary>
    /// <param name="searchText">Поисковая строка.</param>
    /// <returns>Список резюме после поиска.</returns>
    public async Task<ResumeResultOutput> SearchResumesAsync(string searchText)
    {
        try
        {
            var result = new ResumeResultOutput();
            var resumes = await _resumeRepository.GetFilterResumesAsync();
            
            // Получаем все резюме из БД без выгрузки в память.
            ResumesDocumentLoader.Load(resumes, _index, _analyzer);
            
            using var reader = IndexReader.Open(_index.Value, true);
            using var searcher = new IndexSearcher(reader);
            var queryParser = new QueryParser(_version, ResumeFinderConst.SEARCH_FIELD, _analyzer);
            var query = queryParser.Parse(searchText);

            // Больше 20 и не надо, так как есть пагинация.
            var searchResults = searcher.Search(query, 20).ScoreDocs;
            var items = CreateResumesSearchResultBuilder.CreateResumesSearchResult(searchResults, searcher);
            result.CatalogResumes = items;

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}