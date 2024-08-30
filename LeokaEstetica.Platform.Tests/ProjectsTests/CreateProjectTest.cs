using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Enums;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class CreateProjectTest : BaseServiceTest
{
    [Test]
    public async Task CreateProjectTestAsync()
    {
        var createProjectInput = CreateProjectInputFactory();
        var result = await ProjectService.CreateProjectAsync(createProjectInput);
        
        Assert.IsNotNull(result);
        Assert.IsTrue(result.ProjectId > 0);
    }

    private CreateProjectInput CreateProjectInputFactory()
    {
        var result = new CreateProjectInput("Тестовый проект 3", "Это просто тестовый проект", null,
            ProjectStageEnum.Concept.ToString(), null,true)
        {
            Account = "sierra_93@mail.ru",
            Conditions = "Тестовые требования",
            Demands = "Тестовые условия",
            
        };

        return result;
    }
}