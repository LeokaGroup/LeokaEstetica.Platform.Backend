using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class GetModerationRolesTest : BaseServiceTest
{
    [Test]
    public async Task GetModerationRoleAsyncTest()
    {
        var result = await PgContext.ModerationUsers
            .Include(u => u.User)
            .Include(u => u.ModerationRole)
            .Select(u => new
            {
                u.Id,
                ModerationUserId = u.UserId,
                u.DateCreated,
                u.UserRoleId,
                MainUserId = u.User.UserId,
                u.User.Email,
                u.ModerationRole.RoleId
            })
            .FirstOrDefaultAsync();
        
        Assert.NotNull(result);
    }
}