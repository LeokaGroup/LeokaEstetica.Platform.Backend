using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using LeokaEstetica.Platform.Models.Entities.Project;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class CatalogProjectTest : BaseServiceTest
{
    [Test]
    public async Task GetCatalogProjectAsyncTest()
    {
        await AddProjectInArchiveForTestAsync();
        var catalog = await ProjectService.CatalogProjectsAsync();

        var result = catalog.CatalogProjects.FirstOrDefault(c => c.ProjectId == 223);

        Assert.IsTrue(result is null);
    }

    private async Task AddProjectInArchiveForTestAsync()
    {
        var archived = await PgContext.ArchivedProjects.FirstOrDefaultAsync(a => a.ProjectId == 223);
        var testProject = new ArchivedProjectEntity
        {
            ProjectId = 223,
            UserId = 32,
        };

        if (archived is null)
        {
            PgContext.ArchivedProjects.Add(testProject);
            
        }
        else
        {
            PgContext.ArchivedProjects.Remove(archived);
            PgContext.ArchivedProjects.Add(testProject);
        }

        await PgContext.SaveChangesAsync();
    }
}
