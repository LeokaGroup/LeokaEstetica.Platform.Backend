using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.PressTests;

[TestFixture]
internal class GetPublicOfferTest : BaseServiceTest
{
    [Test]
    public async Task GetPublicOfferAsyncTest()
    {
        var result = await PressService.GetPublicOfferAsync();
        
        Assert.NotNull(result);
        Assert.IsNotEmpty(result);
    }
}