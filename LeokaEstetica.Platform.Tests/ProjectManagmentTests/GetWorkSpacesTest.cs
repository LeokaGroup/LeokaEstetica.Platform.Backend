using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class GetWorkSpacesTest : BaseServiceTest
{
    [Test]
    public async Task GetWorkSpacesAsync()
    {
        var result = await ProjectManagmentService.GetWorkSpacesAsync("sierra_93@mail.ru");
        
        Assert.NotNull(result);
    }
}