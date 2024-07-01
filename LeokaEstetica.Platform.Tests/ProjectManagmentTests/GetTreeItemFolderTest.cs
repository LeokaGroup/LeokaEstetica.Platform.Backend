using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetTreeItemFolderTest : BaseServiceTest
{
    [Test]
    public async Task GetTreeItemFolderAsyncTest()
    {
        var result = await WikiTreeService.GetTreeItemFolderAsync(274, 1);
        
        Assert.NotNull(result);
        Assert.IsNotEmpty(result);
    }
}