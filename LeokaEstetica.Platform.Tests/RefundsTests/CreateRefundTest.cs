using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.RefundsTests;

[TestFixture]
internal class CreateRefundTest : BaseServiceTest
{
    [Test]
    public async Task CreateRefundAsyncTest()
    {
        var result = await RefundsService.CreateRefundAsync(3, 50, "sierra_93@mail.ru", null);
        
        IsNotNull(result);
        That(result.Status, Is.EqualTo("Success"));
    }
}