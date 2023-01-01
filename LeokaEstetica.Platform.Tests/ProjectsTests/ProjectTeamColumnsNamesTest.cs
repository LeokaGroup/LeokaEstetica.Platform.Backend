using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class ProjectTeamColumnsNamesTest : BaseServiceTest
{
    [Test]
    public async Task ProjectTeamColumnsNamesAsyncTest()
    {
        var result = await ProjectService.ProjectTeamColumnsNamesAsync();

        Assert.IsNotEmpty(result);
    }
}