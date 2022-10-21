using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ProfileTests;

[TestFixture]
public class ProfileIntentsTest : BaseServiceTest
{
    [Test]
    public async Task ProfileIntentsAsyncTest()
    {
        var result = await ProfileService.ProfileIntentsAsync();
        
        IsTrue(result.Any());
    }
}