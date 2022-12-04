using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
public class UpdateVacancyTest : BaseServiceTest
{
    [Test]
    public async Task UpdateVacancyAsyncTest()
    {
        var result = await VacancyService.UpdateVacancyAsync("test", "Тестовое описание вакансии",
            "Без опыта", "Свободная", "Без оплаты", "sierra_93@mail.ru", 1);
        
        Assert.IsNotNull(result);
        Assert.Positive(result.VacancyId);
    }
}