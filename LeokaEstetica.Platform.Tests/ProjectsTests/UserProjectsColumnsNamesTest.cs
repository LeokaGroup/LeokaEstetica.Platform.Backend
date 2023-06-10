using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class UserProjectsColumnsNamesTest : BaseServiceTest
{
    [Test]
    public async Task UserProjectsColumnsNamesAsyncTest()
    {
        var result = await ProjectService.UserProjectsColumnsNamesAsync();
        
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
    }
}