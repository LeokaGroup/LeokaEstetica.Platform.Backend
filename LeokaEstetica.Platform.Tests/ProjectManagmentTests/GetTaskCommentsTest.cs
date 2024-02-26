using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetTaskCommentsTest : BaseServiceTest
{
    [Test]
    public async Task GetTaskCommentsAsyncTest()
    {
        var result = await ProjectManagmentService.GetTaskCommentsAsync("TE-4", 274);
        
        Assert.NotNull(result);
        Assert.IsNotEmpty(result);
        Assert.That(result.All(x => x.CommentId > 0));
        Assert.That(result.All(x => !string.IsNullOrWhiteSpace(x.Comment)));
    }
}