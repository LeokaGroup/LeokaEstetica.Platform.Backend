using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetAvailableProjectSprintsTest : BaseServiceTest
{
    [Test]
    public async Task GetAvailableProjectSprintsAsyncTest()
    {
        var result = await ProjectManagmentService.GetAvailableProjectSprintsAsync(274, "TE-8");
        
        Assert.NotNull(result);
        Assert.IsNotEmpty(result);
    }
}