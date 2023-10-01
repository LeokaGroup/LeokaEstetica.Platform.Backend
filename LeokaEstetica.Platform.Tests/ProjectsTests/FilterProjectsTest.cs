using LeokaEstetica.Platform.Models.Dto.Input.Project;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class FilterProjectsTest : BaseServiceTest
{
    [Test]
    public async Task FilterProjectsAsyncTest()
    {
        var result = await ProjectService.FilterProjectsAsync(new FilterProjectInput
        {
            Date = "Date",
            IsAnyVacancies = true,
            StageValues = "Concept"
        });
        
        NotNull(result);
        IsTrue(result.Any());
    }
}