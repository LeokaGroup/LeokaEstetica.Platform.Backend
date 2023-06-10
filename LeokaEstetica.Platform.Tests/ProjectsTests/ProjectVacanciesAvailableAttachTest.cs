using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class ProjectVacanciesAvailableAttachTest : BaseServiceTest
{
    [Test]
    public async Task ProjectVacanciesAvailableAttachAsyncTest()
    {
        var result = await ProjectService.ProjectVacanciesAvailableAttachAsync(21, "sierra_93@mail.ru");
        
        Assert.NotNull(result);
    }
}