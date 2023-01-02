using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class SearchInviteProjectMembersTest : BaseServiceTest
{
    [Test]
    public async Task SearchInviteProjectMembersAsyncSuccessTest()
    {
        var result = await ProjectFinderService.SearchInviteProjectMembersAsync("alisaiva931@mail.ru");
        
        Assert.IsNotEmpty(result);
    }
    
    [Test]
    public async Task SearchInviteProjectMembersAsyncErrorTest()
    {
        var result = await ProjectFinderService.SearchInviteProjectMembersAsync("testuser");
        
        Assert.IsEmpty(result);
    }
}