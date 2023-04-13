using LeokaEstetica.Platform.Models.Entities.Vacancy;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class VacanciesModerationTest : BaseServiceTest
{
    [Test]
    public async Task VacanciesModerationAsyncTest()
    {
        await AddVacanciesInArchiveForTestAsync();

        var result = await VacancyModerationService.VacanciesModerationAsync();

        Assert.IsNotEmpty(result.Vacancies);
        Assert.IsNull(result.Vacancies.FirstOrDefault(v => v.VacancyId == 299));
    }

    private async Task AddVacanciesInArchiveForTestAsync()
    {
        var archived = PgContext.ArchivedVacancies
            .Include(v=>v.UserVacancy)
            .FirstOrDefault(a => a.VacancyId == 299);
        var testVacancy = new ArchivedVacancyEntity
        {
            VacancyId = 299,
            UserId = 32,
            UserVacancy = archived.UserVacancy,
        };

        if (archived is null)
        {
            PgContext.ArchivedVacancies.Add(testVacancy);

        }
        else
        {
            PgContext.ArchivedVacancies.Remove(archived);
            PgContext.ArchivedVacancies.Add(testVacancy);
        }

        await PgContext.SaveChangesAsync();
    }
}