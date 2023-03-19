using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.LandingTests;

[TestFixture]
public class GetLandingKnowledgeTest : BaseServiceTest
{
    [Test]
    public async Task GetLandingKnowledgeAsyncTest()
    {
        var result = await KnowledgeService.GetLandingKnowledgeAsync();

        NotNull(result);
        IsNotEmpty(result.KnowledgeLanding);
        NotNull(result.Title);
        IsNotEmpty(result.Title);
    }
}