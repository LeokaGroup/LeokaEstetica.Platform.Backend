using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
internal class GetVacanciesPaginationTest : BaseServiceTest
{
    [Test]
    public async Task GetVacanciesPaginationAsyncTest()
    {
        var result = await VacancyPaginationService.GetVacanciesPaginationAsync(1);
        
        IsNotNull(result);
        IsNotEmpty(result.Vacancies);
    }
}