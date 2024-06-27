using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetProjectInvitesTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectInvitesAsyncTest()
    {
        var result = await ProjectNotificationsService.GetProjectInvitesAsync(274);
        
        Assert.NotNull(result);
    }
}