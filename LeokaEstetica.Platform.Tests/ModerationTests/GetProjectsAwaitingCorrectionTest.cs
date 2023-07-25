using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
internal class GetProjectsAwaitingCorrectionTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectsAwaitingCorrectionAsyncTest()
    {
        var result = await ProjectModerationRepository.GetProjectsAwaitingCorrectionAsync();
        Assert.NotNull(result);
    }
}