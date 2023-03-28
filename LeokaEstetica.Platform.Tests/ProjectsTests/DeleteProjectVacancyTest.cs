using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class DeleteProjectVacancyTest : BaseServiceTest
{
    [Test]
    public async Task DeleteProjectVacancyAsyncTest()
    {
        await ProjectService.DeleteProjectVacancyAsync(28, 21, "sierra_93@mail.ru", string.Empty);
    }
}