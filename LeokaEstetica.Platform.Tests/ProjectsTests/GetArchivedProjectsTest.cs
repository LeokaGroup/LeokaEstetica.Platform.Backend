﻿using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectsTests;

[TestFixture]
internal class GetArchivedProjectsTest : BaseServiceTest
{
    [Test]
    public async Task GetArchivedProjectsAsyncTest()
    {
        var result = await PgContext.ArchivedProjects.ToListAsync();

        Assert.NotNull(result);
    }
}
