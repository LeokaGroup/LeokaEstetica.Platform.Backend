using System.Text;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.IntegrationsTests;

[TestFixture]
internal class SendErrorMessageTelegramBotTest : BaseServiceTest
{
    [Test]
    public async Task SendErrorMessageAsyncTest()
    {
        try
        {
            var testEx = new InvalidOperationException("test exception telegram bot");
            throw testEx;
        }
        
        catch (Exception ex)
        {
            var errorMessage = new StringBuilder();
            errorMessage.Append("[Develop]");
            errorMessage.Append("\nErrorMessage: ");
            errorMessage.Append(ex.Message);
            errorMessage.Append("\nStackTrace:\n");
            errorMessage.Append(ex.StackTrace);

            Assert.DoesNotThrowAsync(async () =>
                await TelegramBotService.SendErrorMessageAsync(errorMessage.ToString()));
            
            await Task.CompletedTask;
        }
    }
}