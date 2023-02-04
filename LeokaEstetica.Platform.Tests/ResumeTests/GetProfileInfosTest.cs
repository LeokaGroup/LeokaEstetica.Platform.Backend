using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ResumeTests;

[TestFixture]
public class GetProfileInfosTest : BaseServiceTest
{
    [Test]
    public async Task GetProfileInfosAsync()
    {
        var result = await ResumeService.GetProfileInfosAsync();
        
        Assert.IsNotEmpty(result);
    }
}