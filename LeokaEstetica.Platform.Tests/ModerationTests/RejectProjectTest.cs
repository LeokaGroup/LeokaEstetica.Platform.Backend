using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
internal class RejectProjectTest : BaseServiceTest
{
    [Test]
    public async Task RejectProjectAsyncTest()
    {
        var result = await ProjectModerationService.RejectProjectAsync(28, "sierra_93@mail.ru");
        
        Assert.IsTrue(result.IsSuccess);
    }
}