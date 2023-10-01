using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class SearchProjectsTest : BaseServiceTest
{
    [Test]
    public async Task SearchProjectsAsyncTest()
    {
        var result = await FinderProjectService.SearchProjectsAsync("Тестовый проект");
        
        IsNotNull(result);
        IsNotEmpty(result.CatalogProjects);
    }
}