using LeokaEstetica.Platform.Models.Dto.Input.Moderation;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
internal class CreateResumeRemarksTest : BaseServiceTest
{
    [Test]
    public async Task CreateResumeRemarksAsyncTest()
    {
        var resumeRemarks = CreateResumeRemarksRequest();
        var result = await ResumeModerationService.CreateResumeRemarksAsync(resumeRemarks, "sierra_93@mail.ru", null);
        var items = result.ToList();
        
        IsNotEmpty(items);
        IsNotNull(items);
        True(items.All(p => p.ProfileInfoId > 0));
    }
    
    private CreateResumeRemarkInput CreateResumeRemarksRequest()
    {
        var result = new CreateResumeRemarkInput()
        {
            ResumesRemarks = new List<ResumeRemarkInput>
            {
                new()
                {
                    ProfileInfoId = 1,
                    FieldName = "RemarkEmail",
                    RemarkText = "Не заполнена почта.",
                    RussianName = "Почта пользователя"
                }
            }
        };

        return result;
    }
}