using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class GetProjectRemarksTest : BaseServiceTest
{
    [Test]
    public Task GetProjectRemarksAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () => await ProjectService.GetProjectRemarksAsync(213, "sierra_93@mail.ru"));
        
        return Task.CompletedTask;
    }
}