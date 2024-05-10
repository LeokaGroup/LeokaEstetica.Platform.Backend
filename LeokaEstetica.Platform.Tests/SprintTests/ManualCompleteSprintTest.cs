using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.SprintTests;

[TestFixture]
internal class ManualCompleteSprintTest : BaseServiceTest
{
    [Test]
    public async Task ManualCompleteSprintAsyncTest()
    {
        var result = await SprintService.ManualCompleteSprintAsync(new ManualCompleteSprintInput
        {
            ProjectSprintId = 3,
            ProjectId = 274
        }, "sierra_93@mail.ru", null);
        
        Assert.NotNull(result);
    }
}