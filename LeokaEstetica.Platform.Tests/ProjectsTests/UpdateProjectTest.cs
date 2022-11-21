using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class UpdateProjectTest : BaseServiceTest
{
    [Test]
    public async Task UpdateProjectAsyncTest()
    {
        var result = await ProjectService.UpdateProjectAsync("Новое название проекта1", "Новое описание проекта", "sierra_93@mail.ru", 5);
        
        IsNotNull(result);
        IsNotEmpty(result.ProjectName);
        IsNotEmpty(result.ProjectDetails);
        Positive(result.ProjectId);
    }
}