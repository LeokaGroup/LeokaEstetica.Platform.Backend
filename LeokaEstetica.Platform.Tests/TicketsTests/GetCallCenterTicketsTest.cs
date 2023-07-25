using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.TicketsTests;

[TestFixture]
internal class GetCallCenterTicketsTest : BaseServiceTest
{
    [Test]
    public async Task SuccessGetCallCenterTicketsAsyncTest()
    {
        Assert.DoesNotThrowAsync(async() => await TicketService.GetCallCenterTicketsAsync("sierra_93@mail.ru"));
        
        await Task.CompletedTask;
    }
    
    [Test]
    public async Task FailGetCallCenterTicketsAsyncTest()
    {
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await TicketService.GetCallCenterTicketsAsync("alisaiva931@mail.ru"));

        await Task.CompletedTask;
    }
}