using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class WriteProjectResponseTest : BaseServiceTest
{
    [Test]
    public async Task WriteProjectResponseAsyncTest()
    {
        var result = await ProjectService.WriteProjectResponseAsync(55, 2, "sierra_93@mail.ru", string.Empty);

        Assert.NotNull(result);
    }
}