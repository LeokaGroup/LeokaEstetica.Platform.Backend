using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace LeokaEstetica.Platform.Tests.ModerationTests;

[TestFixture]
public class GetVacancyModerationByVacancyIdTest : BaseServiceTest
{
    [Test]
    public async Task GetVacancyModerationByVacancyIdAsyncTest()
    {
        var result = await VacancyModerationService.GetVacancyModerationByVacancyIdAsync(9);
        
        IsNotNull(result);
        IsTrue(result.VacancyId > 0);
    }
}