using LeokaEstetica.Platform.Models.Enums;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

/// <summary>
/// Класс тестирует создание обычной связи задачи.
/// </summary>
[TestFixture]
internal class CreateTaskLinkDefaultTest : BaseServiceTest
{
    [Test]
    public Task CreateTaskLinkDefaultAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () =>
            await ProjectManagmentService.CreateTaskLinkDefaultAsync(1, 2, LinkTypeEnum.Link, 274,
                "sierra_93@mail.ru"));
                
        return Task.CompletedTask;
    }
}