using LeokaEstetica.Platform.Models.Enums;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ChatTests;

[TestFixture]
internal class ProjectDialogTest : BaseServiceTest
{
    /// <summary>
    /// Метод тестирует создание нового диалога в проекте, поэтому не передаем Id диалога.
    /// </summary>
    [Test]
    public async Task CreateProjectDialogAsync()
    {
        var result = await ChatService.GetDialogAsync(null, DiscussionTypeEnum.Project, "sierra_93@mail.ru", 21);

        Assert.IsNotNull(result);
    }
}