using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
public class CreateVacancyTest : BaseServiceTest
{
    [Test]
    public async Task CreateVacancyAsyncTest()
    {
        var result = await VacancyService.CreateVacancyAsync(new VacancyInput
        {
            VacancyName = "Тестовая вакансия",
            VacancyText = "Тестовое описание вакансии",
            WorkExperience = "Без опыта",
            Payment = "Без оплаты",
            Account = "sierra_93@mail.ru",
            Employment = "Свободная",
            ProjectId = 60
        });

        Assert.IsNotNull(result);
        Assert.Positive(result.VacancyId);
    }
}