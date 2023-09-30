using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class ProjectVacanciesAvailableAttachTest : BaseServiceTest
{
    [Test]
    public async Task ProjectVacanciesAvailableAttachAsyncTest()
    {
        var result = await ProjectService.ProjectVacanciesAvailableAttachAsync(274, "sierra_93@mail.ru", false);
        
        Assert.NotNull(result);
    }
    
    [Test]
    public async Task ProjectVacanciesAvailableInviteAsyncTest()
    {
        var result = await ProjectService.ProjectVacanciesAvailableAttachAsync(274, "sierra_93@mail.ru", true);
        
        Assert.NotNull(result);
    }
}