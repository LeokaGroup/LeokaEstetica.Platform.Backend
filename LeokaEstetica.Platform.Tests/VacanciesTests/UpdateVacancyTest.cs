using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
internal class UpdateVacancyTest : BaseServiceTest
{
    [Test]
    public async Task UpdateVacancyAsyncTest()
    {
        var result = await VacancyService.UpdateVacancyAsync(
            new VacancyInput
            {
                WorkExperience = "Без опыта",
                Payment = "Без оплаты",
                Account = "sierra_93@mail.ru",
                Employment = "Свободная",
                VacancyName = "Тестовая вакансия",
                VacancyText = "Тестовое описание вакансии"
            });

        Assert.IsNotNull(result);
        Assert.Positive(result.VacancyId);
    }
}