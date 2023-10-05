using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.SearchTests;

[TestFixture]
internal class SearchSkillsByNameTest : BaseServiceTest
{
    [Test]
    public async Task SearchSkillsByNameAsyncTest()
    {
        var result = await SearchProfileService.SearchSkillsByNameAsync("Angular".ToLower());
        
        Assert.NotNull(result);
        Assert.IsNotEmpty(result);
    }
    
    [Test]
    public async Task SearchPartialSkillsByNameAsyncTest()
    {
        var result = await SearchProfileService.SearchSkillsByNameAsync("ang".ToLower());
        
        Assert.NotNull(result);
        Assert.IsNotEmpty(result);
    }
}