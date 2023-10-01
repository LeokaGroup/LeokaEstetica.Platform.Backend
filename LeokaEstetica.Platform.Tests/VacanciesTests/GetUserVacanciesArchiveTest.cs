using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
internal class GetUserVacanciesArchiveTest : BaseServiceTest
{
    [Test]
    public async Task GetUserVacanciesArchiveAsyncTest()
    {
        var result = await PgContext.ArchivedVacancies.ToListAsync();

        Assert.NotNull(result);
    }
}