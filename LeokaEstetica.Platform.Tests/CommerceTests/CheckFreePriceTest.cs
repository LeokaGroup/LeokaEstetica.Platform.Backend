using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.CommerceTests;

[TestFixture]
internal class CheckFreePriceTest : BaseServiceTest
{
    [Test]
    public async Task CheckFreePriceAsyncTest()
    {
        var guid = new Guid("0f9e23c8-338d-47fc-8a0f-3e539d98615c");
        var result = await CommerceService.CheckFreePriceAsync("sierra_93@mail.ru", guid, 2);
        
        That(result.FreePrice, Is.Positive);
    }
}