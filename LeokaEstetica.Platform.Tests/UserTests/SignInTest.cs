using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.UserTests;

[TestFixture]
public class SignInTest : BaseServiceTest
{
    [Test]
    public async Task SignInAsyncTest()
    {
        var result = await UserService.SignInAsync("sierra_93@mail.ru", "12345");
        
        Assert.IsNotNull(result);
        Assert.IsTrue(!result.Errors.Any());
        Assert.IsNotNull(result.Token);
    }
}