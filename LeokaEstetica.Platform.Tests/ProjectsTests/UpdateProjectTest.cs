using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Enums;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class UpdateProjectTest : BaseServiceTest
{
    [Test]
    public async Task UpdateProjectAsyncTest()
    {
        var updateProjectInput = CreateUpdateProjectFactory();

        var result = await ProjectService.UpdateProjectAsync(updateProjectInput);

        IsNotNull(result);
        IsNotEmpty(result.ProjectName);
        IsNotEmpty(result.ProjectDetails);
        Positive(result.ProjectId);
    }

    /// <summary>
    /// Метод создает входную модель для обновления проекта.
    /// </summary>
    /// <returns>Наполненная модель для обновления проекта.</returns>
    private UpdateProjectInput CreateUpdateProjectFactory()
    {
        var result = new UpdateProjectInput("Новое название проекта1", "Новое описание проекта", 5,
            ProjectStageEnum.Concept.ToString(), null, true)
        {
            Account = "sierra_93@mail.ru",
            Conditions = "Тестовые условия тест",
            Demands = "Тестовые требования тест"
        };

        return result;
    }
}