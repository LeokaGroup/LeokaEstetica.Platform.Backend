using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class UserProjectsTest : BaseServiceTest
{
    [Test]
    public async Task UserProjectsAsyncTest()
    {
        var result = await ProjectService.UserProjectsAsync("sierra_93@mail.ru");
        // Assert.IsNotEmpty(result);
    }
}