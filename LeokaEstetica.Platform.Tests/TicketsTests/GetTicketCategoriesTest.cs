using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.TicketsTests;

[TestFixture]
internal class GetTicketCategoriesTest : BaseServiceTest
{
    [Test]
    public async Task GetTicketCategoriesAsyncTest()
    {
        var result = await TicketService.GetTicketCategoriesAsync();
        
        Assert.That(result.Any());
    }
}