using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class ProjectVacanciesTest : BaseServiceTest
{
    [Test]
    public async Task ProjectVacanciesAsyncTest()
    {
        var result = await ProjectService.ProjectVacanciesAsync(21);

        Assert.IsNotNull(result);

        // if (result.ProjectVacancies.Any())
        // {
        //     Assert.Positive(result.ProjectVacancies.First().VacancyId);
        // }
    }
}