using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class SendProjectRemarksTest : BaseServiceTest
{
    [Test]
    public Task SendProjectRemarksAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () => await ProjectModerationService.SendProjectRemarksAsync(213,
            "sierra_93@mail.ru", null));
        
        return Task.CompletedTask;
    }
}