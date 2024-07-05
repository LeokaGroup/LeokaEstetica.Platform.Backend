using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetUserRolesTest : BaseServiceTest
{
    [Test]
    public async Task GetUserRolesAsyncTest()
    {
        var result = await ProjectManagmentRoleService.GetUserRolesAsync("sierra_93@mail.ru");
        
        Assert.NotNull(result);
        Assert.IsNotEmpty(result);
    }
}