using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
public class GetUserVacanciesTest : BaseServiceTest
{
    [Test]
    public async Task GetUserVacanciesAsyncTest()
    {
        var result = await VacancyService.GetUserVacanciesAsync("sierra_93@mail.ru");

        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result.Vacancies);
    }
}