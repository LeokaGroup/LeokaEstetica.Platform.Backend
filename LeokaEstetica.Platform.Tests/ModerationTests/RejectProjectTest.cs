using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class RejectProjectTest : BaseServiceTest
{
    [Test]
    public async Task RejectProjectAsyncTest()
    {
        var result = await ProjectModerationService.RejectProjectAsync(28);
        
        Assert.IsTrue(result.IsSuccess);
    }
}