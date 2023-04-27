using LeokaEstetica.Platform.Models.Dto.Input.Moderation;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class CreateProjectRemarksTest : BaseServiceTest
{
    [Test]
    public async Task CreateProjectRemarksAsyncTest()
    {
        var projectRemarks = CreateProjectRemarksRequest();
        var result = await ProjectModerationService.CreateProjectRemarksAsync(projectRemarks, "sierra_93@mail.ru", 
            null);

        var items = result.ToList();
        
        IsNotEmpty(items);
        IsNotNull(items);
        True(items.All(p => p.ProjectId > 0));
    }

    private CreateProjectRemarkInput CreateProjectRemarksRequest()
    {
        var result = new CreateProjectRemarkInput()
        {
            ProjectRemarks = new List<ProjectRemarkInput>
            {
                new()
                {
                    ProjectId = 214,
                    FieldName = "RemarkProjectName",
                    RemarkText = "Название проекта не соответствует действительности. Заполните его более объективно.",
                    RussianName = "Название проекта"
                },
                
                new()
                {
                    ProjectId = 214,
                    FieldName = "RemarkProjectDetails",
                    RemarkText = "Описание проекта не соответствует действительности. Заполните его более объективно.",
                    RussianName = "Описание проекта"
                }
            }
        };

        return result;
    }
}