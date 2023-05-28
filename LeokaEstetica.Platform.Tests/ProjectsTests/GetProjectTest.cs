using LeokaEstetica.Platform.Access.Enums;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class GetProjectTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectAsyncTest()
    {
        var result = await ProjectService.GetProjectAsync(10, ModeEnum.Edit, "sierra_93@mail.ru");
        
        Assert.IsNotNull(result);
        Assert.Positive(result.ProjectId);
    }
}