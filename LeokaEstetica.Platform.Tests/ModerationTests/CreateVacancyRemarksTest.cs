using LeokaEstetica.Platform.Models.Dto.Input.Moderation;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class CreateVacancyRemarksTest : BaseServiceTest
{
    [Test]
    public async Task CreateVacancyRemarksAsyncTest()
    {
        var vacancyRemarks = CreateVacancyRemarksRequest();
        var result = await VacancyModerationService.CreateVacancyRemarksAsync(vacancyRemarks, "sierra_93@mail.ru", 
            null);

        var items = result.ToList();
        
        IsNotEmpty(items);
        IsNotNull(items);
        True(items.All(p => p.VacancyId > 0));
    }
    
    private CreateVacancyRemarkInput CreateVacancyRemarksRequest()
    {
        var result = new CreateVacancyRemarkInput()
        {
            VacanciesRemarks = new List<VacancyRemarkInput>
            {
                new()
                {
                    VacancyId = 264,
                    FieldName = "RemarkVacancyName",
                    RemarkText = "Название вакансии не соответствует действительности. Заполните его более объективно.",
                    RussianName = "Название вакансии"
                },
                
                new()
                {
                    VacancyId = 264,
                    FieldName = "RemarkVacancyDetails",
                    RemarkText = "Описание вакансии не соответствует действительности. Заполните его более объективно.",
                    RussianName = "Описание вакансии"
                }
            }
        };

        return result;
    }
}