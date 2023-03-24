using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
public class ArchivedVacancyMappingTest : BaseServiceTest
{
    [Test]
    public async Task CheckOfRecipientFromTheArchivedVacanciesAsyncTest()
    {
        var archivedVacancies = PgContext.ArchivedVacancies.ToList();
    }
}