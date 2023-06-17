using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class AddProjectArchiveTest : BaseServiceTest
{
    [Test]
    public Task AddProjectArchiveAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () => await ProjectService.AddProjectArchiveAsync(270, "sierra_93@mail.ru",
            null));
        
        return Task.CompletedTask;
    }
}