using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
internal class AddUserBlackListTest : BaseServiceTest
{
    [Test]
    public async Task AddUserBlackListAsyncTest()
    {
        await UserBlackListService.AddUserBlackListAsync(46, "qwerty123@bk.ru", null);
    }
}