using LeokaEstetica.Platform.Models.Dto.Input.Ticket;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.TicketsTests;

[TestFixture]
internal class CreateTicketTest : BaseServiceTest
{
    [Test]
    public async Task SuccessCreateTicketAsyncTest()
    {
        var request = await CreateTicketRequest();
        
        Assert.DoesNotThrowAsync(async () =>
            await TicketService.CreateTicketAsync(request.Title, request.Message, "alisaiva931@mail.ru"));
    }

    [Test]
    public async Task FailCreateTicketAsyncTest()
    {
        var request = await CreateTicketRequest();
        
        Assert.ThrowsAsync<Exception>(async () =>
            await TicketService.CreateTicketAsync(request.Title, request.Message, "alisaiva931@mail.ru"));
    }

    private async Task<CreateTicketInput> CreateTicketRequest()
    {
        return await Task.FromResult(new CreateTicketInput
        {
            Title = "Вопрос по системе",
            Message = "Как пользоваться вашей системой?"
        });
    }
}