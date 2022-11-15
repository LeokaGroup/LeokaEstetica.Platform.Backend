using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
public class CreateVacancyTest : BaseServiceTest
{
    [Test]
    public async Task CreateVacancyAsyncTest()
    {
        var result = await VacancyService
            .CreateVacancyAsync("Тестовая вакансий", "Тестовое описание вакансии", "Без опыта", "Свободная", "Без оплаты", "sierra_93@mail.ru");

        Assert.IsNotNull(result);
        Assert.Positive(result.VacancyId);
    }
}