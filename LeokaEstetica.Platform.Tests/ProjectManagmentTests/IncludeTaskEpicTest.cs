using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class IncludeTaskEpicTest : BaseServiceTest
{
    [Test]
    public async Task IncludeTaskEpicAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () =>
            await ProjectManagmentService.IncludeTaskEpicAsync(2, 274, "TE-5", "sierra_93@mail.ru"));

        await Task.CompletedTask;
    }
}