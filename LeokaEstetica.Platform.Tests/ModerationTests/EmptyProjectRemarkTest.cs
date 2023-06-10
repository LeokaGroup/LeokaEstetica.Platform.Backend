using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
internal class EmptyProjectRemarkTest : BaseServiceTest
{
    [Test]
    public async Task EmptyProjectRemarkAsyncTest()
    {
        var result = await PgContext.ProjectRemarks.FirstOrDefaultAsync();

        Assert.IsNull(result);
    }
}