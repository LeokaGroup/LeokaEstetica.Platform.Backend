using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class ProjectsModerationTest : BaseServiceTest
{
    [Test]
    public async Task ProjectsModerationAsyncTest()
    {
        var result = await ProjectModerationService.ProjectsModerationAsync();

        IsNotNull(result);
        var moderationProjectEntities = result.Projects.ToList();
        IsNotEmpty(moderationProjectEntities);
        IsTrue(moderationProjectEntities.FirstOrDefault()?.ProjectId > 0);
    }
}