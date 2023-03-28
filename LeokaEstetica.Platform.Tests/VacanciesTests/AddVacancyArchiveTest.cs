using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
public class AddVacancyArchiveTest : BaseServiceTest
{
    [Test]
    public async Task AddVacancyArchiveAsyncTest()
    { 
        var userVacancies = await VacancyService.GetUserVacanciesAsync("sierra_93@mail.ru");
        var vacancies = userVacancies.Vacancies.ToList();
        await VacancyService.AddVacancyArchiveAsync(vacancies[0].VacancyId, "sierra_93@mail.ru");

        var result = PgContext.ArchivedVacancies.FirstOrDefault(a => a.VacancyId == vacancies[0].VacancyId);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.DateArchived);
        Assert.IsTrue(result.VacancyId == vacancies[0].VacancyId);
    }
}