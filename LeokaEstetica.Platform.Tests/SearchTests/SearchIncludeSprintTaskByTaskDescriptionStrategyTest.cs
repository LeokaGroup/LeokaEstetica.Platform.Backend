using LeokaEstetica.Platform.Services.Strategies.ProjectManagement.SprintTaskSearch;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.SearchTests;

[TestFixture]
internal class SearchIncludeSprintTaskByTaskDescriptionStrategyTest : BaseServiceTest
{
    [Test]
    public async Task SearchIncludeSprintTaskByTaskDescriptionStrategyAsyncTest()
    {
        var result = await BaseSearchSprintTaskAlgorithm.SearchAgileObjectByObjectNameAsync(
            new SearchAgileObjectByObjectNameStrategy(ProjectManagmentRepository), "test", 274, 2);

        Assert.IsTrue(result.Any());
    }
}