using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetCompanyProjectUsersTest : BaseServiceTest
{
    [Test]
    public async Task GetCompanyProjectUsersAsyncTest()
    {
        var result = await ProjectManagementSettingsService.GetCompanyProjectUsersAsync(38, "sierra_93@mail.ru");
        
        Assert.NotNull(result);
    }
}