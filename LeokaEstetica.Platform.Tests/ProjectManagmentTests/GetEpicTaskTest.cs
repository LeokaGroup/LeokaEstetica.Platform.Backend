using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetEpicTaskTest : BaseServiceTest
{
    [Test]
    public async Task GetEpicTaskAsyncTest()
    {
        var result = await ProjectManagmentService.GetEpicTaskAsync(274, "TE-1");
        
        Assert.NotNull(result);
    }
}