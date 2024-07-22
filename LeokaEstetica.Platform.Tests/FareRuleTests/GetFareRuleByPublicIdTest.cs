using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.FareRuleTests;

[TestFixture]
internal class GetFareRuleByPublicIdTest : BaseServiceTest
{
    [Test]
    public async Task GetFareRuleByPublicIdAsyncTest()
    {
        var result = await FareRuleService.GetFareRuleByPublicIdAsync(new Guid("a21e861d-ffca-47bb-a342-b4c04a761eca"));
        
        Assert.NotNull(result);
    }
}