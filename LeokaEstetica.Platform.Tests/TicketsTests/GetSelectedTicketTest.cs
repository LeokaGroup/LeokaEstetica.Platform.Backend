using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.TicketsTests;

[TestFixture]
internal class GetSelectedTicketTest : BaseServiceTest
{
    [Test]
    public async Task SuccessGetSelectedTicketAsyncTest()
    {
        Assert.DoesNotThrowAsync(async() => await TicketService.GetSelectedTicketAsync(3, "sierra_93@mail.ru"));
        
        await Task.CompletedTask;
    }
    
    [Test]
    public async Task FailGetSelectedTicketAsyncTest()
    {
        var test = await TicketService.GetSelectedTicketAsync(-1, "sierra_93@mail.ru");
        
        Assert.That(test.TicketId == 0);
        Assert.That(!test.Messages.Any());
        
        await Task.CompletedTask;
    }
    
    [Test]
    public async Task GetSelectedTicketAsyncTest()
    {
        var test = await TicketService.GetSelectedTicketAsync(3, "sierra_93@mail.ru");
        
        Assert.That(test.TicketId > 0);
        Assert.That(test.Messages.Any);
        
        await Task.CompletedTask;
    }
}