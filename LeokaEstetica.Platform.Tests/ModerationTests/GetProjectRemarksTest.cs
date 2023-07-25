using LeokaEstetica.Platform.Database.Repositories.Moderation.Project;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
internal class GetProjectRemarksTest : BaseServiceTest
{
    private readonly ProjectModerationRepository _projectModerationRepository;
    
    public GetProjectRemarksTest()
    {
        _projectModerationRepository = new ProjectModerationRepository(PgContext);
    }
    
    [Test]
    public async Task GetProjectRemarksAsyncCheckFieldMappingTest()
    {
        var remarks = await _projectModerationRepository.GetProjectRemarksAsync(213);
        if (!remarks.Any())
        {
            Assert.Fail("Отсутствуют замечания");
        }

        var testRemark = remarks.FirstOrDefault(r => r.RemarkId == 12);
        if (testRemark is null)
        {
            Assert.Fail("Замечание не найдено");
        }
        
        Assert.That(testRemark.RejectReason, Is.EqualTo("default"));
    }
}