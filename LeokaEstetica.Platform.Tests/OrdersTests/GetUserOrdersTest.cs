using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.OrdersTests;

[TestFixture]
internal class GetUserOrdersTest : BaseServiceTest
{
    [Test]
    public async Task GetUserOrdersAsyncTest()
    {
        var result = await OrdersService.GetUserOrdersAsync("sierra_93@mail.ru");
        var orderEntities = result.ToList();
        
        That(orderEntities, Is.Not.Null);
        Multiple(() =>
        {
            That(orderEntities.Any(), Is.True);
        });
        That(orderEntities.All(o => o.OrderId > 0));
    }
}