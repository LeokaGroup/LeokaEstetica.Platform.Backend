using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ResumeTests;

[TestFixture]
internal class SearchResumesTest : BaseServiceTest
{
    [Test]
    public async Task SearchResumesAsyncTest()
    {
        var result = await ResumeFinderService.SearchResumesAsync("Иванов Иван Иванович");
        
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result.CatalogResumes);
    }
}