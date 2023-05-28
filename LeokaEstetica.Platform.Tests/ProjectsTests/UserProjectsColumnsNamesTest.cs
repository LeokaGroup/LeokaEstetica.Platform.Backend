using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class UserProjectsColumnsNamesTest : BaseServiceTest
{
    [Test]
    public async Task UserProjectsColumnsNamesAsyncTest()
    {
        var result = await ProjectService.UserProjectsColumnsNamesAsync();
        
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
    }
}