using LeokaEstetica.Platform.Models.Dto.Input.Commerce;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Output.Commerce.YandexKassa;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ProcessingTests;

[TestFixture]
internal class CreateOrderTest : BaseServiceTest
{
    [Test]
    public async Task CreateOrderAsyncTest()
    {
        DoesNotThrowAsync(async () => await CommerceService.CreateOrderCacheOrRabbitMqAsync(new CreateOrderInput
        {
            PublicId = new Guid("0f9e23c8-338d-47fc-8a0f-3e539d98615c"),
            PaymentMonth = 2
        }, "sierra_93@mail.ru"));
        
        // var result = await PayMasterService.CreateOrderAsync(new Guid("0f9e23c8-338d-47fc-8a0f-3e539d98615c"),
        //     "sierra_93@mail.ru");

        // NotNull(result);
        //
        // var paymentId = string.Empty;
        //
        // if (result is CreateOrderPayMasterOutput payMasterOutput)
        // {
        //     paymentId = payMasterOutput.PaymentId;
        // }
        //
        // else if (result is CreateOrderYandexKassaOutput yandexKassaOutput)
        // {
        //     paymentId = yandexKassaOutput.PaymentId;
        // }
        
        // IsTrue(long.Parse(paymentId) > 0);
    }
}