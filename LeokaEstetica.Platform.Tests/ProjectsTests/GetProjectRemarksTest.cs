using LeokaEstetica.Platform.Core.Enums;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
public class GetProjectRemarksTest : BaseServiceTest
{
    [Test]
    public async Task GetProjectRemarksAsyncTest()
    {
        var result = await ProjectService.GetProjectRemarksAsync(213, "sierra_93@mail.ru");
        
        Assert.True(result.All(x => x.RemarkStatusId == (int)RemarkStatusEnum.AwaitingCorrection));
    }
}