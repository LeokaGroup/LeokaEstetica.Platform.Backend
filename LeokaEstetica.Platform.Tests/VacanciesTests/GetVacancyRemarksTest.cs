using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
public class GetVacancyRemarksTest : BaseServiceTest
{
    [Test]
    public Task GetVacancyRemarksAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () => await VacancyService.GetVacancyRemarksAsync(303, "sierra_93@mail.ru"));
        
        return Task.CompletedTask;
    }
}