using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ProfileTests;

[TestFixture]
internal class ProfileIntentsTest : BaseServiceTest
{
    [Test]
    public async Task ProfileIntentsAsyncTest()
    {
        var result = await ProfileService.ProfileIntentsAsync(string.Empty);
        
        IsTrue(result.Any());
    }
}