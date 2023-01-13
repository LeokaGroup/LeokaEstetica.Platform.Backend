using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
public class SearchVacanciesTest : BaseServiceTest
{
    [Test]
    public async Task SearchVacanciesAsyncTest()
    {
        var result = await VacancyFinderService.SearchVacanciesAsync("Тестовая вакансия");
        
        IsNotNull(result);
        IsNotEmpty(result.CatalogVacancies);
    }
}