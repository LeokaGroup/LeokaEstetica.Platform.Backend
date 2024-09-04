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
        // Assert.Positive(result.VacancyId);
    }
}