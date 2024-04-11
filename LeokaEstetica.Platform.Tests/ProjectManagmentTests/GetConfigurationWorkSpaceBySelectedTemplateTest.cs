using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetConfigurationWorkSpaceBySelectedTemplateTest : BaseServiceTest
{
    [Test]
    public async Task GetConfigurationWorkSpaceBySelectedTemplateAsyncTest()
    {
        var result = await ProjectManagmentService.GetConfigurationWorkSpaceBySelectedTemplateAsync(295, "kn", 1,
            "sierra_93@mail.ru");

        Assert.NotNull(result);
        Assert.IsNotEmpty(result.ProjectManagmentTaskStatuses);
        Assert.True(result.ProjectManagmentTaskStatuses.Any());
    }
}