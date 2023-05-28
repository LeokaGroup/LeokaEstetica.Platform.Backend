using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class ApproveProjectTest : BaseServiceTest
{
    [Test]
    public async Task ApproveProjectAsyncTest()
    {
        var result = await ProjectModerationService.ApproveProjectAsync(28, "sierra_93@mail.ru");
        
        Assert.IsTrue(result.IsSuccess);
    }
}