using LeokaEstetica.Platform.Models.Enums;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class CreateProjectTest : BaseServiceTest
{
    [Test]
    public async Task CreateProjectTestAsync()
    {
        var result = await ProjectService.CreateProjectAsync("Тестовый проект 3", "Это просто тестовый проект",
            "sierra_93@mail.ru", ProjectStageEnum.Concept, null, "Тестовые требования", "Тестовые условия");
        
        Assert.IsNotNull(result);
        Assert.IsTrue(result.ProjectId > 0);
    }
}