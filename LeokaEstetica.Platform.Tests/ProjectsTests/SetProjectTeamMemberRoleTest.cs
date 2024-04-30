using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class SetProjectTeamMemberRoleTest : BaseServiceTest
{
    [Test]
    public async Task SetProjectTeamMemberRoleAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () => await ProjectService.SetProjectTeamMemberRoleAsync(32, "Владелец", 274));

        await Task.CompletedTask;
    }
}