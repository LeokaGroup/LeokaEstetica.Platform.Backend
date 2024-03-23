using LeokaEstetica.Platform.Services.Strategies.ProjectManagement.SprintTaskSearch;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.SearchTests;

[TestFixture]
internal class SearchIncludeSprintTaskByTaskDescriptionStrategyTest : BaseServiceTest
{
    [Test]
    public async Task SearchIncludeSprintTaskByTaskDescriptionStrategyAsyncTest()
    {
        var result = await BaseSearchSprintTaskAlgorithm.SearchIncludeSprintTaskByTaskDescriptionAsync(
            new SearchIncludeSprintTaskByTaskDescriptionStrategy(ProjectManagmentRepository), "test", 274, 2);

        Assert.IsTrue(result.Any());
    }
}