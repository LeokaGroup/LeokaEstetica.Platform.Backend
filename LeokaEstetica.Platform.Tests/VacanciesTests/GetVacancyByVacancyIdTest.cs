using LeokaEstetica.Platform.Access.Enums;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
internal class GetVacancyByVacancyIdTest : BaseServiceTest
{
    [Test]
    public async Task GetVacancyByVacancyIdAsyncTest()
    {
        var result = await VacancyService.GetVacancyByVacancyIdAsync(2, ModeEnum.View, "sierra_93@mail.ru");
        
        Assert.NotNull(result);
        Assert.Positive(result.VacancyId);
    }

    [Test]
    public async Task GetVacancyByVacancyIdAsyncForbiddenTest()
    {
        var result = await VacancyService.GetVacancyByVacancyIdAsync(2, ModeEnum.Edit, "test1@mail.ru");

        Assert.IsFalse(result.IsAccess);
    }
}