using LeokaEstetica.Platform.Access.Enums;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class GetProjectTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectAsyncTest()
    {
        var result = await ProjectService.GetProjectAsync(260, ModeEnum.Edit, "sierra_93@mail.ru");
        
        Assert.IsNotNull(result);
        Assert.Positive(result.ProjectId);
    }
}