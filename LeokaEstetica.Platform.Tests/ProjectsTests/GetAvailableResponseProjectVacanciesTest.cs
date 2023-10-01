using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class GetAvailableResponseProjectVacanciesTest : BaseServiceTest
{
    [Test]
    public async Task GetAvailableResponseProjectVacanciesAsyncTest()
    {
        var result = await ProjectService.GetAvailableResponseProjectVacanciesAsync(202, "alisaiva931@mail.ru");
        var projectVacancyEntities = result.ToList();
        
        Assert.NotNull(result);
        Assert.IsNotEmpty(projectVacancyEntities);
        Assert.True(projectVacancyEntities.All(x => x.VacancyId > 0));
        Assert.True(projectVacancyEntities.All(x => x.ProjectId > 0));
    }
}