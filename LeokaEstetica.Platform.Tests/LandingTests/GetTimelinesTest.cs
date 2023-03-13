using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.LandingTests;

[TestFixture]
public class GetTimelinesTest : BaseServiceTest
{
    [Test]
    public async Task GetTimelinesAsyncTest()
    {
        var result = await LandingService.GetTimelinesAsync();

        Assert.NotNull(result);
        Assert.IsNotEmpty(result);
    }
}