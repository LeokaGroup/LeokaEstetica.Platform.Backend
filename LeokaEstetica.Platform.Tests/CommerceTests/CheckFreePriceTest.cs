using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.CommerceTests;

[TestFixture]
internal class CheckFreePriceTest : BaseServiceTest
{
    [Test]
    public async Task CheckFreePriceAsyncTest()
    {
        var result = await CommerceService.CheckFreePriceAsync("sierra_93@mail.ru");
        
        That(result.FreePrice, Is.Positive);
    }
}