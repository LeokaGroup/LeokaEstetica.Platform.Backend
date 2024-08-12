using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class WriteProjectResponseTest : BaseServiceTest
{
    [Test]
    public async Task WriteProjectResponseAsyncTest()
    {
        var result = await ProjectService.WriteProjectResponseAsync(55, 2, "sierra_93@mail.ru");

        Assert.NotNull(result);
    }
}