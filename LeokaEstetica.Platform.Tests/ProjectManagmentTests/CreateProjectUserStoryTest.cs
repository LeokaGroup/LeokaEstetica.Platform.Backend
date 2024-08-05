using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using NUnit.Framework;

namespace LeokaEstetica.Platform.Tests.ProjectManagmentTests;

[TestFixture]
internal class CreateProjectUserStoryTest : BaseServiceTest
{
    /// <summary>
    /// Тестирует с опциональными параметрами.
    /// </summary>
    [Test]
    public async Task CreateProjectUserStoryWithOptionalParamsAsyncTest()
    {
        var request = CreateProjectTaskRequestWithOptionalParams();
        
        Assert.DoesNotThrowAsync(async () =>
            await ProjectManagmentService.CreateProjectTaskAsync(request, "sierra_93@mail.ru"));
        
        await Task.CompletedTask;
    }
    
    /// <summary>
    /// Тестирует без опциональных параметров.
    /// </summary>
    [Test]
    public async Task CreateProjectUserStoryWithoutOptionalParamsAsyncTest()
    {
        var request = CreateProjectTaskRequestWithoutOptionalParams();
        
        Assert.DoesNotThrowAsync(async () =>
            await ProjectManagmentService.CreateProjectTaskAsync(request, "sierra_93@mail.ru"));
            
        await Task.CompletedTask;
    }
    
    /// <summary>
    /// Метод создает тестовые данные для создания истории с опциональными параметрами.
    /// </summary>
    /// <returns>Тестовые данные.</returns>
    private CreateProjectManagementTaskInput CreateProjectTaskRequestWithOptionalParams()
    {
        var task = new CreateProjectManagementTaskInput
        {
            Name = "Тестовая история 1",
            Details = "Тестовое описание тестовой истории 1.",
            IsQuickCreate = false,
            PriorityId = (int)TaskPriorityEnum.Medium,
            ProjectId = 274,
            TagIds = new[] { 1, 2 },
            TaskStatusId = (int)ProjectUserStoryStatusEnum.New,
            TaskTypeId = (int)ProjectTaskTypeEnum.Story,
            WatcherIds = new[] { (long)125 },
            EpicId = 8 // В БД дева есть такой эпик.
        };

        return task;
    }
    
    /// <summary>
    /// Метод создает тестовые данные для создания истории без опциональных параметров.
    /// </summary>
    /// <returns>Тестовые данные.</returns>
    private CreateProjectManagementTaskInput CreateProjectTaskRequestWithoutOptionalParams()
    {
        var task = new CreateProjectManagementTaskInput
        {
            Name = "Тестовая история 1",
            Details = "Тестовое описание тестовой истории 1.",
            IsQuickCreate = false,
            PriorityId = (int)TaskPriorityEnum.Medium,
            ProjectId = 274,
            TagIds = new[] { 1, 2 },
            TaskStatusId = (int)ProjectUserStoryStatusEnum.New,
            TaskTypeId = (int)ProjectTaskTypeEnum.Story,
            WatcherIds = new[] { (long)125 },
            EpicId = 8 // В БД дева есть такой эпик.
        };

        return task;
    }
}