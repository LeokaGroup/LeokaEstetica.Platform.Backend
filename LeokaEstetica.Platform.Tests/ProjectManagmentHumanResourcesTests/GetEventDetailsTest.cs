using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentHumanResourcesTests;

[TestFixture]
internal class GetEventDetailsTest : BaseServiceTest 
{
    [Test]
    public async Task GetEventDetailsAsyncTest()
    {
        var result = await CalendarService.GetEventDetailsAsync(4);
        
        Assert.NotNull(result);
    }
}