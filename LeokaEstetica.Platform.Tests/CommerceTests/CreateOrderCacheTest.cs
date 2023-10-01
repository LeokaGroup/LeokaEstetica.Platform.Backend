using LeokaEstetica.Platform.Models.Dto.Input.Commerce;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.CommerceTests;

[TestFixture]
internal class CreateOrderCacheTest : BaseServiceTest
{
    [Test]
    public Task CreateOrderCacheAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () => await CommerceService.CreateOrderCacheAsync(new CreateOrderCacheInput
        {
            PublicId = new Guid("0f9e23c8-338d-47fc-8a0f-3e539d98615c"),
            PaymentMonth = 2
        }, "sierra_93@mail.ru"));

        return Task.CompletedTask;
    }
}