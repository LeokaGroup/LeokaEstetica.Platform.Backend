using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.IntegrationsTests;

[TestFixture]
internal class CreateNotificationsChanelInviteLinkTest : BaseServiceTest
{
    [Test]
    public async Task CreateNotificationsChanelInviteLinkAsyncTest()
    {
        var result = await TelegramService.CreateNotificationsChanelInviteLinkAsync();
        
        Assert.NotNull(result);
    }
}