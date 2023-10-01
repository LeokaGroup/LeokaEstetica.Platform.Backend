using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.OrdersTests;

[TestFixture]
internal class GetOrderDetailsTest : BaseServiceTest
{
    [Test]
    public async Task GetOrderDetailsAsyncTest()
    {
        var result = await OrdersService.GetOrderDetailsAsync(3, "sierra_93@mail.ru");
        
        That(result, Is.Not.Null);
        
        Multiple(() =>
        {
            That(result.OrderId > 0, Is.True);
        });
        
        That(result.OrderDetails, Is.Not.Null);
    }
}