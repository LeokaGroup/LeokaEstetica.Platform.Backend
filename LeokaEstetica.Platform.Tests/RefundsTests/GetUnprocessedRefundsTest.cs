using LeokaEstetica.Platform.Base.Enums;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.RefundsTests;

[TestFixture]
internal class GetUnprocessedRefundsTest : BaseServiceTest
{
    [Test]
    public async Task GetUnprocessedRefundsTestAsync()
    {
        var result = await RefundService.GetUnprocessedRefundsAsync();
        
        Assert.NotNull(result);
        Assert.NotNull(result.All(x => x.Status.Equals(RefundStatusEnum.Pending.ToString())));
    }
}