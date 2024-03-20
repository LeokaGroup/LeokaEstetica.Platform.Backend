using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class PlaningSprintTest : BaseServiceTest
{
    [Test]
    public async Task PlaningSprintAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () => await ProjectManagmentService.PlaningSprintAsync(new PlaningSprintInput
        {
            SprintName = "Тестовый спринт 1",
            SprintDescription = "Тестовый спринт 1 тестовое описание",
            DateStart = DateTime.UtcNow,
            DateEnd = DateTime.UtcNow.AddDays(4),
            ProjectId = 274,
            ProjectTaskIds = new [] { "TE-1", "TE-2", "TE-3" }
        }, "sierra_93@mail.ru", null));

        await Task.CompletedTask;
    }
}