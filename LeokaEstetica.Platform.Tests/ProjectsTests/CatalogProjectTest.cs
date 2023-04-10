using NUnit.Framework;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class CatalogProjectTest : BaseServiceTest
{
    [Test]
    public async Task GetCatalogProjectAsyncTest()
    {
        var catalog = await ProjectService.CatalogProjectsAsync();

        var result = catalog.CatalogProjects
            .Where((n => PgContext.ArchivedProjects.Any(t => t.ProjectId == n.ProjectId)))
            .ToList();

        Assert.IsTrue(result.Count == 0);
    }
}
