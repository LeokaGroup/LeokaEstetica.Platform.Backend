using LeokaEstetica.Platform.Models.Entities.Vacancy;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
public class CatalogVacanciesTest : BaseServiceTest
{
    [Test]
    public async Task GetCatalogVacanciesAsyncTest()
    {
        await AddVacanciesInArchiveForTestAsync();
        var catalog = await ProjectService.CatalogProjectsAsync();

        var result = catalog.CatalogProjects.FirstOrDefault(c => c.ProjectId == 223);

        Assert.IsTrue(result is null);
    }

    private async Task AddVacanciesInArchiveForTestAsync()
    {
        var archived = PgContext.ArchivedVacancies.FirstOrDefault(a => a.VacancyId == 266);
        var testVacancy = new ArchivedVacancyEntity
        {
            VacancyId = 266,
            UserId = 32,
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
