using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class GetProjectUnShippedRemarksTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectUnShippedRemarksAsyncTest()
    {
        var result = await ProjectModerationService.GetProjectUnShippedRemarksAsync(209);
        
        NotNull(result);
        IsNotEmpty(result);
    }
}