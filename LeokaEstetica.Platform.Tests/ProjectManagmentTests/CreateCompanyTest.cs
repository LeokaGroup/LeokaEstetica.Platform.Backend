using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class CreateCompanyTest : BaseServiceTest
{
    [Test]
    public Task CreateCompany()
    {
        Assert.DoesNotThrowAsync(async () =>
            await CompanyService.CreateCompanyAsync("test company", "sierra_93@mail.ru"));

        return Task.CompletedTask;
    }
}