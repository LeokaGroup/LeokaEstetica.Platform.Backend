using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
internal class CreateVacancyTest : BaseServiceTest
{
    [Test]
    public async Task CreateVacancyAsyncTest()
    {
        var result = await VacancyService.CreateVacancyAsync(
            new VacancyInput("Тестовая вакансия", "Тестовое описание вакансии", null, 60, null)
            {
                WorkExperience = "Без опыта",
                Payment = "Без оплаты",
                Account = "sierra_93@mail.ru",
                Employment = "Свободная"
            });

        Assert.IsNotNull(result);
        Assert.Positive(result.VacancyId);
    }
}