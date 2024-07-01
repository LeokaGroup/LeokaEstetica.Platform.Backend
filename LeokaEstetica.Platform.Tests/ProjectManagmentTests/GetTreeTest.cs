using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetTreeTest : BaseServiceTest
{
    [Test]
    public async Task GetTreeAsyncTest()
    {
        var result = await WikiTreeService.GetTreeAsync(274);

        Assert.NotNull(result);
        Assert.IsNotEmpty(result);
    }
}