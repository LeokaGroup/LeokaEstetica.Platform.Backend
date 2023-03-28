using LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Processing.Enums;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ProcessingTests;

[TestFixture]
public class CreateOrderTest : BaseServiceTest
{
    [Test]
    public async Task CreateOrderAsyncTest()
    {
        var result = await PayMasterService.CreateOrderAsync(new CreateOrderInput
        {
            Amount = new Amount
            {
                Currency = PaymentCurrencyEnum.RUB.ToString(),
                Value = 299
            },
            FareRuleId = 2,
            Invoice = new Invoice
            {
                Description = "Оплата тарифа \"Тариф Базовый\""
            }
        }, "sierra_93@mail.ru", string.Empty);

        NotNull(result);
        IsTrue(long.Parse(result.PaymentId) > 0);
    }
}