﻿using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.VacanciesTests;

[TestFixture]
internal class AddVacancyArchiveTest : BaseServiceTest
{
    [Test]
    public async Task AddVacancyArchiveAsyncTest()
    { 
        await VacancyService.AddVacancyArchiveAsync(147, "sierra_93@mail.ru");

    }
}