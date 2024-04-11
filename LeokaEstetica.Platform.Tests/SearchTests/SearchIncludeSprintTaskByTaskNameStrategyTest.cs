using LeokaEstetica.Platform.Services.Strategies.ProjectManagement.AgileObjectSearch;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.SearchTests;

/// <summary>
/// Класс тестирует поиск задач по названию задачи для включения их в спринт.
/// </summary>
[TestFixture]
internal class SearchIncludeSprintTaskByTaskNameStrategyTest : BaseServiceTest
{
    [Test]
    public async Task SearchIncludeSprintTaskByTaskNameStrategyAsyncTest()
    {
        var result = await BaseSearchSprintTaskAlgorithm.SearchAgileObjectByObjectDescriptionAsync(
            new SearchAgileObjectByObjectDescriptionStrategy(ProjectManagmentRepository), "тестовая задача", 274, 2);

        Assert.IsTrue(result.Any());
    }
}