using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class CreateTaskCommentTest : BaseServiceTest
{
    [Test]
    public async Task CreateTaskCommentAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () =>
            await ProjectManagmentService.CreateTaskCommentAsync("TE-4", 274, "Тестовый комментарий задачи",
                "sierra_93@mail.ru"));

        await Task.CompletedTask;
    }
}