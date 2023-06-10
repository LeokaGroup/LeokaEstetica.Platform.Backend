using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.SubscriptionTests;

[TestFixture]
internal class GetSubscriptionsTest : BaseServiceTest
{
    [Test]
    public async Task GetSubscriptionsAsyncTest()
    {
        var result = await SubscriptionService.GetSubscriptionsAsync();

        IsNotEmpty(result);
        True(result.All(x => x.ObjectId > 0 && x.SubscriptionId > 0));
    }
}