using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class GetProjectTeamTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectTeamAsyncTest()
    {
        var result = await ProjectService.GetProjectTeamAsync(21);

        Assert.IsNotEmpty(result);
        Assert.IsNotNull(result);
    }
}