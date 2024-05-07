using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetSprintsTest : BaseServiceTest
{
    [Test]
    public async Task GetSprintsAsyncTest()
    {
        var result = await SprintService.GetSprintsAsync(274);
        
        Assert.IsNotNull(result);
    }
}