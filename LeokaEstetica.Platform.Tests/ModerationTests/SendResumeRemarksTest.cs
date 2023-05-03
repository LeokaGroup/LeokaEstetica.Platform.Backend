using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class SendResumeRemarksTest : BaseServiceTest
{
    [Test]
    public Task SendResumeRemarksAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () => await ResumeModerationService.SendResumeRemarksAsync(1, null));
        
        return Task.CompletedTask;
    }
}