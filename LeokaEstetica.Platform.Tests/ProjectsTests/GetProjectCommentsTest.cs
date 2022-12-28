using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class GetProjectCommentsTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectCommentsAsyncTest()
    {
        var result = await ProjectCommentsService.GetProjectCommentsAsync(21);

        Assert.IsNotNull(result);
    }
}