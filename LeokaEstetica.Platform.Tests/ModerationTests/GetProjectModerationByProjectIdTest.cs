using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
internal class GetProjectModerationByProjectIdTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectModerationByProjectIdAsyncTest()
    {
        var result = await ProjectModerationService.GetProjectModerationByProjectIdAsync(21);

        IsNotNull(result);
        IsTrue(result.ProjectId > 0);
    }
}