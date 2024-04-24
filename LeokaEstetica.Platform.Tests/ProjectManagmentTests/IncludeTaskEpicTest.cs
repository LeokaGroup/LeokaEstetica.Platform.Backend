using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class IncludeTaskEpicTest : BaseServiceTest
{
    [Test]
    public async Task IncludeTaskEpicAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () =>
            await ProjectManagmentService.IncludeTaskEpicAsync(2, new[] { "TE-5" }, "sierra_93@mail.ru", string.Empty));

        await Task.CompletedTask;
    }
}