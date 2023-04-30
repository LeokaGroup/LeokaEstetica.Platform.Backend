using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class SendVacancyRemarksTest : BaseServiceTest
{
    [Test]
    public Task SendVacancyRemarksAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () => await VacancyModerationService.SendVacancyRemarksAsync(264,
            "sierra_93@mail.ru", null));
        
        return Task.CompletedTask;
    }
}