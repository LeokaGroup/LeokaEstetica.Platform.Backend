using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class ResumesModerationTest : BaseServiceTest
{
    [Test]
    public async Task ResumesModerationAsyncTest()
    {
        var result = await ResumeModerationService.ResumesModerationAsync();

        Assert.NotNull(result);
        Assert.IsTrue(result.Resumes.Any());
    }
}