using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.MetricsTests;

[TestFixture]
internal class GetLastProjectCommentsTest : BaseServiceTest
{
    [Test]
    public async Task GetLastProjectCommentsAsyncTest()
    {
        var result = await ProjectMetricsService.GetLastProjectCommentsAsync();

        Assert.That(result.Count() <= 5);
    }
}