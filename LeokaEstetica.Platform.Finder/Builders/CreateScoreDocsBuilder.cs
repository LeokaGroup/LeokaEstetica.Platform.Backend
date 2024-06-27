using LeokaEstetica.Platform.Finder.Consts;
using Lucene.Net.Search;

namespace LeokaEstetica.Platform.Finder.Builders;

/// <summary>
/// Класс билдера создает последовательность документов для пагинации.
/// </summary>
public static class CreateScoreDocsBuilder
{
    private static readonly List<ScoreDoc> _scoreDocs = new();

    /// <summary>
    /// Метод создает список документов для пагинации.
    /// </summary>
    /// <param name="page">Номер страницы.</param>
    /// <param name="searcher">Поисковый индекс.</param>
    /// <param name="rowsCount">Кол-во записей, которые надо брать (все записи).</param>
    /// <returns>Список документов.</returns>
    public static ScoreDoc[] CreateScoreDocsResult(int page, IndexSearcher searcher, int rowsCount)
    {
        _scoreDocs.Clear();
        
        var skipRows = (page - 1) * PaginationConst.TAKE_COUNT;
        var searchResults = searcher.Search(new MatchAllDocsQuery(), skipRows + rowsCount)
            .ScoreDocs;

        var result = searchResults
            .Skip(skipRows)
            .Take(PaginationConst.TAKE_COUNT)
            .ToArray();
        
        return result;
    }
}