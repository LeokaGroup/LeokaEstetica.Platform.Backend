using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Enums;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

/// <summary>
/// Класс тестирует создание связи задачи.
/// </summary>
[TestFixture]
internal class CreateTaskLinkDefaultTest : BaseServiceTest
{
    /// <summary>
    /// Тестирует создание обычной связи.
    /// </summary>
    /// <returns></returns>
    [Test]
    public Task CreateTaskLinkDefaultAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () =>
            await ProjectManagmentService.CreateTaskLinkAsync(new TaskLinkInput
            {
                TaskFromLink = "TE-1",
                TaskToLink = "TE-2",
                LinkType = LinkTypeEnum.Link,
                ProjectId = 272
            }, "sierra_93@mail.ru"));

        return Task.CompletedTask;
    }
}