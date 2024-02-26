using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class UpdateTaskCommentTest : BaseServiceTest
{
    [Test]
    public async Task UpdateTaskCommentAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () =>
            await ProjectManagmentService.UpdateTaskCommentAsync("TE-4", 274, 2, "Новый тестовый комментарий",
                "sierra_93@mail.ru"));

        await Task.CompletedTask;
    }
}