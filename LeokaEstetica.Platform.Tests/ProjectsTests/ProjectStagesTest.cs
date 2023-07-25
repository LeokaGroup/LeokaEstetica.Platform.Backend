using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class ProjectStagesTest : BaseServiceTest
{
    [Test]
    public async Task ProjectStagesAsyncTest()
    {
        var result = await ProjectService.ProjectStagesAsync();

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
    }
}