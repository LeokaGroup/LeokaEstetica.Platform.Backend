using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
internal class VacanciesModerationTest : BaseServiceTest
{
    [Test]
    public async Task VacanciesModerationAsyncTest()
    {
        var result = await VacancyModerationService.VacanciesModerationAsync();

        Assert.IsNotEmpty(result.Vacancies);
    }
}