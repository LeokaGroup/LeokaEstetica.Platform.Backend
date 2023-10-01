using LeokaEstetica.Platform.Models.Dto.Input.Commerce;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ProcessingTests;

[TestFixture]
internal class CreateOrderTest : BaseServiceTest
{
    [Test]
    public async Task CreateOrderAsyncTest()
    {
        DoesNotThrowAsync(async () => await CommerceService.CreateOrderCacheAsync(new CreateOrderCacheInput
        {
            PublicId = new Guid("0f9e23c8-338d-47fc-8a0f-3e539d98615c"),
            PaymentMonth = 2
        }, "sierra_93@mail.ru"));
        
        var result = await PayMasterService.CreateOrderAsync(new Guid("0f9e23c8-338d-47fc-8a0f-3e539d98615c"),
            "sierra_93@mail.ru", string.Empty);

        NotNull(result);
        IsTrue(long.Parse(result.PaymentId) > 0);
    }
}