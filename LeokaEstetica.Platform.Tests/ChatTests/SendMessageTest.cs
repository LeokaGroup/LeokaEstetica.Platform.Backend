using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ChatTests;

[TestFixture]
internal class SendMessageTest : BaseServiceTest
{
    [Test]
    public async Task SendMessageAsyncTest()
    {
        var result = await ChatService.SendMessageAsync("Тестовое сообщение", 4, 32, null, true);

        IsNotNull(result);
        IsTrue(result.Messages.Count > 0);
    }
}