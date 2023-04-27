using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class ApproveVacancyTest : BaseServiceTest
{
    /// <summary>
    /// Успешный тест кейс
    /// </summary>
    [Test]
    public async Task ApproveVacancyAsyncTest()
    {
        var result = await VacancyModerationService.ApproveVacancyAsync(299, null);
        
        Assert.IsTrue(result.IsSuccess);
    }
    
    /// <summary>
    /// Тест-кейс с ID несуществующей вакансии
    /// </summary>
    [Test]
    public void ApproveVacancyAsyncThrowInvalidOperationExceptionTest()
    {
        Assert.ThrowsAsync<InvalidOperationException>(async () => await VacancyModerationService.ApproveVacancyAsync(15, null));
    }
}