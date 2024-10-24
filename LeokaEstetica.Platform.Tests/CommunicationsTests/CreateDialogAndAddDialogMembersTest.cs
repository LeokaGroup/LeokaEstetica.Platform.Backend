using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.CommunicationsTests;

[TestFixture]
internal class CreateDialogAndAddDialogMembersTest : BaseServiceTest
{
    [Test]
    public async Task CreateDialogAndAddDialogMembersAsyncTest()
    {
        var result = await AbstractGroupDialogService.CreateDialogAndAddDialogMembersAsync(new List<string>
        {
            "sierra_93@mail.ru",
            "test1@mail.ru"
        }, "test_dialog", "sierra_93@mail.ru", null, null);
        
        Assert.NotNull(result.Dialog);
        Assert.True(result.IsSuccess);
    }
}