using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
internal class GetProjectCommentsModerationTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectCommentsModerationAsyncTest()
    {
        var comments = await ProjectModerationRepository.GetProjectCommentsModerationAsync();
        Assert.That(comments.Any);
    }
}