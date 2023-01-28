using LeokaEstetica.Platform.Finder.Consts;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using Lucene.Net.Search;

namespace LeokaEstetica.Platform.Finder.Builders;

/// <summary>
/// Класс билдера результата резюме.
/// </summary>
public static class CreateResumesSearchResultBuilder
{
    /// <summary>
    /// Метод создает результат поиска резюме.
    /// </summary>
    /// <param name="searchResults">Результаты поиска.</param>
    /// <param name="searcher">Поисковый индекс.</param>
    /// <returns>Список резюме.</returns>
    public static List<ResumeOutput> CreateResumesSearchResult(ScoreDoc[] searchResults,
        IndexSearcher searcher)
    {
        var resumes = new List<ResumeOutput>(20);

        foreach (var item in searchResults)
        {
            var document = searcher.Doc(item.Doc);
            var lastName = string.Empty;
            var firstName = string.Empty;
            var patronymic = string.Empty;
            var job = string.Empty;
            long userId = 0;
            var aboutMe = string.Empty;

            if (!string.IsNullOrEmpty(document.GetField(ResumeFinderConst.LAST_NAME).ToString()))
            {
                lastName = document.GetField(ResumeFinderConst.LAST_NAME).StringValue;
            }

            if (!string.IsNullOrEmpty(document.GetField(ResumeFinderConst.FIRST_NAME).ToString()))
            {
                firstName = document.GetField(ResumeFinderConst.FIRST_NAME).StringValue;
            }

            if (!string.IsNullOrEmpty(document.GetField(ResumeFinderConst.PATRONYMIC).ToString()))
            {
                patronymic = document.GetField(ResumeFinderConst.PATRONYMIC).StringValue;
            }

            if (!string.IsNullOrEmpty(document.GetField(ResumeFinderConst.JOB).ToString()))
            {
                job = document.GetField(ResumeFinderConst.JOB).StringValue;
            }
            
            if (long.Parse(document.GetField(ResumeFinderConst.USER_ID).StringValue) > 0)
            {
                userId = long.Parse(document.GetField(ResumeFinderConst.USER_ID).StringValue);
            }
            
            if (!string.IsNullOrEmpty(document.GetField(ResumeFinderConst.ABOUT_ME).ToString()))
            {
                aboutMe = document.GetField(ResumeFinderConst.ABOUT_ME).StringValue;
            }

            var isShortFirstName = bool.Parse(document.GetField(ResumeFinderConst.IS_SHORT_FIRST_NAME).StringValue);

            resumes.Add(new ResumeOutput
            {
                LastName = lastName,
                FirstName = firstName,
                Patronymic = patronymic,
                Job = job,
                IsShortFirstName = isShortFirstName,
                ProfileInfoId = userId,
                Aboutme = aboutMe
            });
        }

        return resumes;
    }
}