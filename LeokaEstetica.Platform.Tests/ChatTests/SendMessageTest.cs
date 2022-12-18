using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ChatTests;

[TestFixture]
public class SendMessageTest : BaseServiceTest
{
    [Test]
    public async Task SendMessageAsyncTest()
    {
        var result = await ChatService.SendMessageAsync("Тестовое сообщение", 4, "alisaiva931@mail.ru");

        IsNotNull(result);
        IsTrue(result.Messages.Count > 0);
    }
}