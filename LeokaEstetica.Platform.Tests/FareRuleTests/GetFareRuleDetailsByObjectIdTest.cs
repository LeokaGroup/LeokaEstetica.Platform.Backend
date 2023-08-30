using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.FareRuleTests;

[TestFixture]
internal class GetFareRuleDetailsByObjectIdTest : BaseServiceTest
{
    [Test]
    public async Task GetFareRuleDetailsByObjectIdAsyncTest()
    {
        var result = await FareRuleRepository.GetFareRuleDetailsByObjectIdAsync(1);
        
        Assert.IsNotNull(result);
        Assert.True(result.RuleId > 0);
    }
}