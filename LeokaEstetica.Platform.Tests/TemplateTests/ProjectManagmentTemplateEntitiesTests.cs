using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.TemplateTests;

/// <summary>
/// Класс тестов проверяющий все сущности шаблонов модуля УП на корректность их работы с БД.
/// Делаются простые выборки из БД, чтобы проверить, что ни одна из сущностей не упадет при работе с ней.
/// </summary>
[TestFixture]
internal class ProjectManagmentTemplateEntitiesTests : BaseServiceTest
{
    [Test]
    public async Task ProjectManagmentTemplateEntitiesAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () => 
            await PgContext.ProjectManagmentTaskTemplates.Take(3).ToListAsync());
            
        Assert.DoesNotThrowAsync(async () => 
        await PgContext.ProjectManagmentTaskStatusTemplates.Take(3).ToListAsync());
        
        Assert.DoesNotThrowAsync(async () => 
            await PgContext.ProjectManagmentUserTaskTemplates.Take(3).ToListAsync());

        await Task.CompletedTask;
    }
}