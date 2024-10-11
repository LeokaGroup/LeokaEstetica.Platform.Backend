using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.CommunicationsTests;

[TestFixture]
internal class GetAbstractScopesTest : BaseServiceTest
{
    [Test]
    public async Task GetAbstractScopesAsyncTest()
    {
        var result = await AbstractScopeService.GetAbstractScopesAsync("sierra_93@mail.ru");
        
        Assert.NotNull(result);
        Assert.IsNotEmpty(result);
        Assert.True(result.All(x => x.AbstractScopeId.HasValue));
        Assert.True(result.All(x => x.UserId > 0));
    }
}