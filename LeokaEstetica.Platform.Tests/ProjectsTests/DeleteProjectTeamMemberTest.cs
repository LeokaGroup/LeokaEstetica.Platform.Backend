using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class DeleteProjectTeamMemberTest : BaseServiceTest
{
    [Test]
    public async Task DeleteProjectTeamMemberAsyncTest()
    {
        await ProjectService.DeleteProjectTeamMemberAsync(246, 96, "sierra_93@mail.ru");
    }
}