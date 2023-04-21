using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class GetProjectCommentsTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectCommentsAsyncTest()
    {
        var result = await ProjectCommentsService.GetProjectCommentsAsync(215);

        Assert.IsNotNull(result);
        Assert.True(result.Any(p => p.ProjectId == 215));
    }
}