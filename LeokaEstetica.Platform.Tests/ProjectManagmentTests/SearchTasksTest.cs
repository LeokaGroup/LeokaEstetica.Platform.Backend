using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class SearchTasksTest : BaseServiceTest
{
    [Test]
    public async Task SearchTaskAsyncTest()
    {
        var result = await SearchProjectManagementService.SearchTaskAsync("Тестовая задача 1", new List<long> { 274 },
            false, true, true);
        
        Assert.NotNull(result);
        Assert.IsNotEmpty(result);
        Assert.AreEqual(result.Count(), 1);
    }
}