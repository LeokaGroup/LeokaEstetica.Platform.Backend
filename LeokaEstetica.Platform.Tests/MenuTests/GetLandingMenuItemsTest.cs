using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.MenuTests;

[TestFixture]
internal class GetLandingMenuItemsTest : BaseServiceTest
{
    [Test]
    public async Task GetLandingMenuItemsAsyncTest()
    {
        var result = await MenuService.GetLandingMenuItemsAsync();
        
        Assert.NotNull(result);
    }
}