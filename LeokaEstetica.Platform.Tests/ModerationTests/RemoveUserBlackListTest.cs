using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests
{
    [TestFixture]
    public class RemoveUserBlackListTest : BaseServiceTest
    {
        [Test]
        public async Task RemoveUserBlackListAsyncTest()
        {
            await UserBlackListService.RemoveUserBlackListAsync(46, "sdasdf");
        }
    }
}
