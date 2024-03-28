using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetEpicTasksTest : BaseServiceTest
{
    [Test]
    public async Task GetEpicTasksAsyncTest()
    {
        var result = await ProjectManagmentService.GetEpicTasksAsync(274, 8, "sierra_93@mail.ru");
        
        Assert.IsTrue(result.EpicTasks.Any());
    }
}