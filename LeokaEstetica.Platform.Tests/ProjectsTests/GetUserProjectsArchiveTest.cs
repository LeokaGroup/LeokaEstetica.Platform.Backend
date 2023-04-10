using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class GetUserProjectsArchiveTest : BaseServiceTest
{
    [Test]
    public async Task GetUserProjectsArchiveAsyncTest()
    {
        var result = await ProjectService.GetUserProjectsArchiveAsync("sierra_93@mail.ru");
        
        Assert.NotNull(result);
        Assert.IsNotEmpty(result.ProjectArchiveOutputs);
    }
}