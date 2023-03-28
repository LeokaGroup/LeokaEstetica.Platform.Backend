using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class ProjectVacanciesTest : BaseServiceTest
{
    [Test]
    public async Task ProjectVacanciesAsyncTest()
    {
        var result = await ProjectService.ProjectVacanciesAsync(21, "sierra_93@mail.ru", string.Empty);

        Assert.IsNotNull(result);
    }
}