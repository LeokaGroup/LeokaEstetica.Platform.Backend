using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Services.Strategies.ProjectManagement.SprintTaskSearch;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.SearchTests;

[TestFixture]
internal class SearchIncludeSprintTaskByTaskDescriptionStrategyTest : BaseServiceTest
{
    [Test]
    public async Task SearchIncludeSprintTaskByTaskDescriptionStrategyAsyncTest()
    {
        var result = await BaseSearchSprintTaskAlgorithm.SearchSearchAgileObjectByTaskDescriptionAsync(
            new SearchAgileObjectByTaskDescriptionStrategy(ProjectManagmentRepository), "test", 274, 2,
            SearchAgileObjectTypeEnum.Sprint);

        Assert.IsTrue(result.Any());
    }
}