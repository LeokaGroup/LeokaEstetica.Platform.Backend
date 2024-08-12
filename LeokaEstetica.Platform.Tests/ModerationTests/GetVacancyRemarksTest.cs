using LeokaEstetica.Platform.Database.Abstractions.Moderation.Vacancy;
using LeokaEstetica.Platform.Database.Repositories.Moderation.Vacancy;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
internal class GetVacancyRemarksTest : BaseServiceTest
{
    private readonly IVacancyModerationRepository _vacancyModerationRepository;

    public GetVacancyRemarksTest()
    {
        _vacancyModerationRepository = new VacancyModerationRepository(PgContext, null);
    }

    [Test]
    public async Task GetVacancyRemarksAsyncCheckFieldMappingTest()
    {
        // Замечание, с полем RejectReason = "test" 
        var remarkId = 11;
        // Вакансия к которой относится замечание
        var vacancyId = 245;

        var vacancyRemarks = await _vacancyModerationRepository.GetVacancyRemarksAsync(vacancyId);
        if (!vacancyRemarks.Any())
        {
            Assert.Fail("Отсутствуют замечания");
        }

        var testRemark = vacancyRemarks.FirstOrDefault(v => v.RemarkId == remarkId);
        if (testRemark is null)
        {
            Assert.Fail("Замечание не найдено");
        }
        
        Assert.That(testRemark.RejectReason, Is.EqualTo("test"));
    }
}