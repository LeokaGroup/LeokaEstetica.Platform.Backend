using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ProfileTests;

[TestFixture]
public class GetProfileInfoTest : BaseServiceTest
{
    [Test]
    public async Task GetProfileInfoAsyncTest()
    {
        var result = await ProfileService.GetProfileInfoAsync("sierra_93@mail.ru");
        
        IsNotNull(result);
        IsTrue(result.UserId > 0);
    }
}