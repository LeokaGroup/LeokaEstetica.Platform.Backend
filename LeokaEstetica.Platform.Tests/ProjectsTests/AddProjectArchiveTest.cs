using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class AddProjectArchiveTest : BaseServiceTest
{
    [Test]
    public async Task AddProjectArchiveAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () => await ProjectService.AddProjectArchiveAsync(270, "sierra_93@mail.ru"));
    }
}