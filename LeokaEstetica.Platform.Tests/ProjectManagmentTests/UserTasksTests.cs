using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class UserTasksTests : BaseServiceTest
{
    /// <summary>
    /// Тест проверяет, не падают ли сущности всех основных таблиц задач.
    /// </summary>
    [Test]
    public Task GetUserTasksAsyncTest()
    {
        Assert.DoesNotThrowAsync(async () =>
        {
            _ = await PgContext.UserTasks.FirstOrDefaultAsync();
        });
        
        Assert.DoesNotThrowAsync(async () =>
        {
            _ = await PgContext.TaskStatuses.FirstOrDefaultAsync();
        });
        
        Assert.DoesNotThrowAsync(async () =>
        {
            _ = await PgContext.TaskRelations.FirstOrDefaultAsync();
        });
        
        Assert.DoesNotThrowAsync(async () =>
        {
            _ = await PgContext.HistoryActions.FirstOrDefaultAsync();
        });
        
        Assert.DoesNotThrowAsync(async () =>
        {
            _ = await PgContext.TaskHistories.FirstOrDefaultAsync();
        });
        
        Assert.DoesNotThrowAsync(async () =>
        {
            _ = await PgContext.TaskComments.FirstOrDefaultAsync();
        });
        
        Assert.DoesNotThrowAsync(async () =>
        {
            _ = await PgContext.TaskTags.FirstOrDefaultAsync();
        });
        
        Assert.DoesNotThrowAsync(async () =>
        {
            _ = await PgContext.TaskTypes.FirstOrDefaultAsync();
        });
        
        Assert.DoesNotThrowAsync(async () =>
        {
            _ = await PgContext.TaskResolutions.FirstOrDefaultAsync();
        });
        
        Assert.DoesNotThrowAsync(async () =>
        {
            _ = await PgContext.TaskDependencies.FirstOrDefaultAsync();
        });

        return Task.CompletedTask;
    }
}