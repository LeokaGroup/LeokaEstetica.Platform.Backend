using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.FareRuleTests;

[TestFixture]
internal class FareRuleTest : BaseServiceTest
{
    [Test]
    public async Task GetFareRuleAsyncTest()
    {
        var result = await FareRuleService.GetFareRulesAsync();
        
        Assert.IsNotEmpty(result);
    }
}