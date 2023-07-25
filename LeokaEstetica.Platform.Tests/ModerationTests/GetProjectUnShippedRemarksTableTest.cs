using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
internal class GetProjectUnShippedRemarksTableTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectUnShippedRemarksTableAsyncTest()
    {
        var result = await ProjectModerationRepository.GetProjectUnShippedRemarksTableAsync();
        
        NotNull(result);
        True(result.All(x => x.ProjectId > 0));
    }
}