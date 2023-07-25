using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.TicketsTests;

[TestFixture]
internal class GetUserProfileTicketsTest : BaseServiceTest
{
    [Test]
    public async Task GetUserProfileTicketsAsyncTest()
    {
        var result = await TicketService.GetUserProfileTicketsAsync("sierra_93@mail.ru");
        
        Assert.That(result.Any);
    }
}