﻿using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
internal class GetArchivedVacanciesTest : BaseServiceTest
{
    [Test]
    public async Task GetArchivedVacanciesAsyncTest()
    {
        var result = await PgContext.ArchivedVacancies.ToListAsync();

        Assert.NotNull(result);
    }
}