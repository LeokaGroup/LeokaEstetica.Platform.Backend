using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class EmptyRemarkStatuseTest : BaseServiceTest
{
    [Test]
    public async Task EmptyRemarkStatuseAsyncTest()
    {
        var result = await PgContext.RemarkStatuses.FirstOrDefaultAsync();

        Assert.IsNull(result);
    }
}