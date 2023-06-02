using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.OrdersTests;

[TestFixture]
public class GetHistoryTest : BaseServiceTest
{
    [Test]
    public async Task GetHistoryAsyncTest()
    {
        var result = await OrdersService.GetHistoryAsync("sierra_93@mail.ru");
        
        NotNull(result);
        That(result.All(t => t.OrderId > 0), Is.True);
    }
}