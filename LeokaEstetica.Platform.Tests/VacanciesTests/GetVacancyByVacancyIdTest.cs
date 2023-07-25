using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
internal class GetVacancyByVacancyIdTest : BaseServiceTest
{
    [Test]
    public async Task GetVacancyByVacancyIdAsyncTest()
    {
        var result = await VacancyService.GetVacancyByVacancyIdAsync(2, "sierra_93@mail.ru");
        
        Assert.NotNull(result);
        Assert.Positive(result.VacancyId);
    }
}