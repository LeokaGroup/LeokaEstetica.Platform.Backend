using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.RefundsTests;

[TestFixture]
internal class CreateRefundTest : BaseServiceTest
{
    [Test]
    public async Task CreateRefundAsyncTest()
    {
        // Перед запуском этого теста, важно убедиться, что подставляем Id заказа, который можно вернуть.
        // Проверять по PaymentId в ПС.
        var result = await RefundsService.CreateRefundAsync(84, 586, "sierra_93@mail.ru");
        
        IsNotNull(result);
        That(result.Status, Is.EqualTo("Success"));
    }
}