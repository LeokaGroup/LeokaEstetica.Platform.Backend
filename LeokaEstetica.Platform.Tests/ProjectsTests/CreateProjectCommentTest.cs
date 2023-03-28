using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class CreateProjectCommentTest : BaseServiceTest
{
    [Test]
    public async Task CreateProjectCommentAsyncTest()
    {
        await ProjectCommentsService.CreateProjectCommentAsync(6, "Тестовый коммент", "alisaiva931@mail.ru",
            string.Empty);
    }
}