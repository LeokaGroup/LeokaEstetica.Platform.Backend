using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class SearchInviteProjectMembersTest : BaseServiceTest
{
    [Test]
    public async Task SearchInviteProjectMembersAsyncSuccessTest()
    {
        var result = await ProjectFinderService
            .SearchInviteProjectMembersAsync("alisaiva931@mail.ru", string.Empty, null);
        
        Assert.NotNull(result);
    }
    
    [Test]
    public async Task SearchInviteProjectMembersAsyncErrorTest()
    {
        var result = await ProjectFinderService
            .SearchInviteProjectMembersAsync("testuser", string.Empty, null);
        
        Assert.NotNull(result);
    }
}