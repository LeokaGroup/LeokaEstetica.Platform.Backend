using LeokaEstetica.Platform.Models.Enums;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ChatTests;

[TestFixture]
internal class WriteProjectDialogOwnerTest : BaseServiceTest
{
    [Test]
    public async Task WriteProjectDialogOwnerAsyncTest()
    {
        var result =
            await ChatService.WriteProjectDialogOwnerAsync(DiscussionTypeEnum.Project, "alisaiva931@mail.ru", 21, null);

        IsNotNull(result);
        IsTrue(result.DialogId > 0);
    }
}