using LeokaEstetica.Platform.Core.Enums;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
internal class GetProjectUnShippedRemarksTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectUnShippedRemarksAsyncTest()
    {
        var result = await ProjectModerationService.GetProjectUnShippedRemarksAsync(209);
        
        NotNull(result);

        var items = result.ToList();
        
        IsNotEmpty(items);

        IsTrue(items.All(x => new[]
            {
                (int)RemarkStatusEnum.AwaitingCorrection,
                (int)RemarkStatusEnum.NotAssigned
            }
            .Contains(x.RemarkStatusId)));
    }
}