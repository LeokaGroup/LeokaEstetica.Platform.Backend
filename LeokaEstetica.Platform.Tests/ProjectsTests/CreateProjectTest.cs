using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Enums;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class CreateProjectTest : BaseServiceTest
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
        var result = new CreateProjectInput
        {
            ProjectName = "Тестовый проект 3",
            ProjectDetails = "Это просто тестовый проект",
            Account = "sierra_93@mail.ru",
            Token = string.Empty,
            ProjectStage = ProjectStageEnum.Concept.ToString(),
            Conditions = "Тестовые требования",
            Demands = "Тестовые условия"
        };

        return result;
    }
}