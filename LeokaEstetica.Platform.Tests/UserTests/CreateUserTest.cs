using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.UserTests;

[TestFixture]
internal class CreateUserTest : BaseServiceTest
{
    [Test]
    public async Task CreateUserAsyncTest()
    {
        var result = await UserService.CreateUserAsync("12345", "test@mail.ru");
        
        Assert.IsNotNull(result);
    }
}