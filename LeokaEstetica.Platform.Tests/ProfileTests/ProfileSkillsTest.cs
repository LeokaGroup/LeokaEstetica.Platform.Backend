using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ProfileTests;

[TestFixture]
internal class ProfileSkillsTest : BaseServiceTest
{
    [Test]
    public async Task ProfileSkillsAsyncTest()
    {
        var result = await ProfileService.ProfileSkillsAsync(string.Empty);
        
        IsTrue(result.Any());
    }
}