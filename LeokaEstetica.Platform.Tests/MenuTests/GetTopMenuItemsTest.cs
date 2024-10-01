using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.MenuTests;

[TestFixture]
internal class GetTopMenuItemsTest : BaseServiceTest
{
    [Test]
    public async Task GetTopMenuItemsAsyncTest()
    {
        var result = await MenuService.GetTopMenuItemsAsync();

        Assert.NotNull(result);
        Assert.IsNotEmpty(result.Items);
    }
}