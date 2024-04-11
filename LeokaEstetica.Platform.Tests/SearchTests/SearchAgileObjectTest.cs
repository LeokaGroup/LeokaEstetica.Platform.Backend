using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.SearchTests;

[TestFixture]
internal class SearchAgileObjectTest : BaseServiceTest
{
    [Test]
    public async Task SearchAgileObjectAsyncByAllConditionsTest()
    {
        var result = await SearchProjectManagementService.SearchAgileObjectAsync("TE-15 Тестовая задача 1", true, true,
            true, 274, "sierra_93@mail.ru");
        
        Assert.NotNull(result);
    }
}