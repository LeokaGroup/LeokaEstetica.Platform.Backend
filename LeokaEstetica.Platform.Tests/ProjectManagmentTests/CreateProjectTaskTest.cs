using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class CreateProjectTaskTest : BaseServiceTest
{
    /// <summary>
    /// Тест создает задачу обычным способом.
    /// </summary>
    [Test]
    public Task CreateProjectTaskAsyncTest()
    {
        var request = CreateProjectTaskRequest();

        Assert.DoesNotThrowAsync(async () =>
        {
            await ProjectManagmentService.CreateProjectTaskAsync(request, "sierra_93@mail.ru");
        });
        
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Тест создает задачу быстрым способом.
    /// </summary>
    [Test]
    public Task CreateQuickProjectTaskAsyncTest()
    {
        var request = CreateQuickProjectTaskRequest();

        Assert.DoesNotThrowAsync(async () =>
        {
            await ProjectManagmentService.CreateProjectTaskAsync(request, "sierra_93@mail.ru");
        });
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Метод создает тестовые данные для обычного создания задачи.
    /// </summary>
    /// <returns></returns>
    private CreateProjectManagementTaskInput CreateProjectTaskRequest()
    {
        var task = new CreateProjectManagementTaskInput
        {
            Name = "Тестовая задача для проверки созданная обычным способом",
            Details = "Тестовое описание тестовой задачи.",
            ExecutorId = 32,
            IsQuickCreate = false,
            PriorityId = (int)TaskPriorityEnum.Medium,
            ProjectId = 274,
            TagIds = new[] { 1, 2 },
            TaskStatusId = (int)ProjectTaskStatusEnum.New,
            TaskTypeId = (int)ProjectTaskTypeEnum.Task,
            WatcherIds = new[] { (long)125 }
        };

        return task;
    }
    
    /// <summary>
    /// Метод создает тестовые данные для быстрого создания задачи.
    /// </summary>
    /// <returns></returns>
    private CreateProjectManagementTaskInput CreateQuickProjectTaskRequest()
    {
        var task = new CreateProjectManagementTaskInput
        {
            Name = "Тестовая задача для проверки созданная быстрым способом",
            IsQuickCreate = true,
            PriorityId = (int)TaskPriorityEnum.Medium,
            ProjectId = 274,
            TaskStatusId = (int)ProjectTaskStatusEnum.New,
            TaskTypeId = (int)ProjectTaskTypeEnum.Task
        };

        return task;
    }
}