using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
internal class SendVacancyRemarksTest : BaseServiceTest
{
    [Test]
    public Task SendVacancyRemarksAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () => await VacancyModerationService.SendVacancyRemarksAsync(264, null));
        
        return Task.CompletedTask;
    }
}