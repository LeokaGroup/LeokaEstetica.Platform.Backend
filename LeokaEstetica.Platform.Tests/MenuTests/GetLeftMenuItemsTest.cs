using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.MenuTests;

[TestFixture]
internal class GetLeftMenuItemsTest : BaseServiceTest
{
    [Test]
    public async Task GetLeftMenuItemsAsyncTest()
    {
        var result = await MenuService.GetLeftMenuItemsAsync();

        Assert.NotNull(result);
        Assert.IsNotEmpty(result.Items);
    }
}