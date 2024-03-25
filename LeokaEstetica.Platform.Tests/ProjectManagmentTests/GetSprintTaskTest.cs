using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetSprintTaskTest : BaseServiceTest
{
    [Test]
    public async Task GetSprintTaskAsyncTest()
    {
        var result = await ProjectManagmentService.GetSprintTaskAsync(274, "TE-4");
        
        Assert.NotNull(result);
    }
}