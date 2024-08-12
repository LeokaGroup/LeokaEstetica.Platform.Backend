using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.SprintTests;

[TestFixture]
internal class StartSprintTest : BaseServiceTest
{
    [Test]
    public async Task StartSprintAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () => await SprintService.StartSprintAsync(3, 274, "sierra_93@mail.ru"));

        var checkSprintStatus = await SprintRepository.GetSprintAsync(3, 274);
        
        Assert.That(checkSprintStatus?.SprintStatusId, Is.EqualTo(2));
    }
}