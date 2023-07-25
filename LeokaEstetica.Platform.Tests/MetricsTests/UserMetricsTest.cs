using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.MetricsTests;

[TestFixture]
internal class UserMetricsTest : BaseServiceTest
{
    [Test]
    public async Task GetNewUsersTest()
    {
        var result = await UserMetricsService.GetNewUsersAsync();
        
        var userEntities = result.ToList();
        That(userEntities, Is.Not.Null);
        That(userEntities, Is.Not.Empty);
    }
}