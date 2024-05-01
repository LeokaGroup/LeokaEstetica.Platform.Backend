using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetSprintTest : BaseServiceTest
{
    [Test]
    public async Task GetSprintAsyncTest()
    {
        var result = await SprintService.GetSprintAsync(1, 274);
        
        Assert.NotNull(result);
        Assert.True(result.ProjectSprintId > 0);
    }
}