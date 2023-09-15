using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.PressTests;

[TestFixture]
internal class GetContactsTest : BaseServiceTest
{
    [Test]
    public async Task GetContactsAsyncTest()
    {
        var result = await PressService.GetContactsAsync();
        
        Assert.NotNull(result);
        Assert.IsNotEmpty(result);
    }
}