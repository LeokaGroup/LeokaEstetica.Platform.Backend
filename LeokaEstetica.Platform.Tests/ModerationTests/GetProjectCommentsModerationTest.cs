using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class GetProjectCommentsModerationTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectModerationByProjectIdAsyncTest()
    {
        var result = await ProjectModerationService.GetProjectCommentsModerationAsync(215);

        Assert.IsNotNull(result);
    }
}