using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.TicketsTests;

[TestFixture]
internal class CreateTicketMessageTest : BaseServiceTest
{
    [Test]
    public async Task CreateTicketMessageAsyncTest()
    {
        var result = await TicketService.CreateTicketMessageAsync(3, "test message", "sierra_93@mail.ru");
        
        That(result.Messages.Any);
        That(result.Messages.All(m => m.MessageId > 0));
    }
}