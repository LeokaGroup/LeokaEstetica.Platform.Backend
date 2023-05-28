using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class GetUsersBlackListTest : BaseServiceTest
{
    [Test]
    public async Task GetUsersBlackListAsyncTest()
    {
        var result = await UserBlackListService.GetUsersBlackListAsync();
        
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result.UsersBlackList);
    }
}