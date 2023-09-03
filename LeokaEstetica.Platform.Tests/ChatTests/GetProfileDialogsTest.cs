using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ChatTests;

[TestFixture]
internal class GetProfileDialogsTest : BaseServiceTest
{
    [Test]
    public async Task GetProfileDialogsAsyncTest()
    {
        var result = await ChatRepository.GetProfileDialogsAsync(125);
        
        Assert.IsNotNull(result);
    }
}