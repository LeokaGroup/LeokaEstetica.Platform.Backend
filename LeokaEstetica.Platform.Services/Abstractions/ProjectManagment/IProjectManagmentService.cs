﻿using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Output.Template;
using LeokaEstetica.Platform.Models.Entities.Document;
using LeokaEstetica.Platform.Models.Entities.Profile;
using LeokaEstetica.Platform.Models.Entities.ProjectManagment;
using LeokaEstetica.Platform.Models.Entities.Template;
using LeokaEstetica.Platform.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;

/// <summary>
/// TODO: Разделить в рамках рефача методы на отдельные сервисы, чтобы этот сервис не был таким толстым.
/// Абстракция сервиса управления проектами.
/// </summary>
public interface IProjectManagmentService
{
    /// <summary>
    /// Метод получает список стратегий представления рабочего пространства.
    /// </summary>
    /// <returns>Список стратегий.</returns>
    Task<IEnumerable<ViewStrategyEntity>> GetViewStrategiesAsync();

    /// <summary>
    /// Метод получает элементы панели.
    /// Панель это и хидер модуля УП и левое выдвижное меню и меню документации проектов.
    /// Этот метод не заполняет доп.списки.
    /// </summary>
    /// <returns>Список элементов.</returns>
    Task<IEnumerable<PanelEntity>> GetPanelItemsAsync();

    /// <summary>
    /// TODO: Код дублируется местами. Если не будет явных расширений в логике, то убрать лишнюю логику.
    /// Метод наполняет доп.списки элементов панели.
    /// </summary>
    /// <param name="items">Список элементов.</param>
    Task<GetPanelResult> ModifyPanelItemsAsync(IEnumerable<PanelOutput> items);

    /// <summary>
    /// Метод получает конфигурацию рабочего пространства по выбранному шаблону.
    /// Под конфигурацией понимаются основные элементы рабочего пространства (набор задач, статусов, фильтров, колонок и тд)
    /// если выбранный шаблон это предполагает.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="paginatorStatusId">Id статуса, для которого нужно применить пагинатор.
    /// Если он null, то пагинатор применится для задач всех статусов шаблона.</param>
    /// <param name="modifyTaskStatuseType">Компонент, данные которого будем модифицировать.</param>
    /// <param name="page">Номер страницы.</param>
    /// <returns>Данные конфигурации рабочего пространства.</returns>
    Task<ProjectManagmentWorkspaceResult> GetConfigurationWorkSpaceBySelectedTemplateAsync(long projectId,
        string account, int? paginatorStatusId, ModifyTaskStatuseTypeEnum modifyTaskStatuseType, int page = 1);

    /// <summary>
    /// Метод получает детали задачи.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskDetailType">Тип детализации.</param>
    /// <returns>Данные задачи.</returns>
    Task<ProjectManagmentTaskOutput> GetTaskDetailsByTaskIdAsync(string projectTaskId, string account, long projectId,
        TaskDetailTypeEnum taskDetailType);

    /// <summary>
    /// Метод получает список типов задач.
    /// </summary>
    /// <returns>Список типов задач.</returns>
    Task<IEnumerable<TaskTypeEntity>> GetTaskTypesAsync();

    /// <summary>
    /// Метод создает задачу проекта.
    /// </summary>
    /// <param name="projectManagementTaskInput">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>
    /// <returns>Выходная модель.</returns>
    Task<CreateProjectManagementTaskOutput> CreateProjectTaskAsync(
        CreateProjectManagementTaskInput projectManagementTaskInput, string account, string? token);

    /// <summary>
    /// Метод получает список приоритетов задачи.
    /// </summary>
    /// <returns>Список приоритетов задачи.</returns>
    Task<IEnumerable<TaskPriorityEntity>> GetTaskPrioritiesAsync();

    /// <summary>
    /// Метод получает список тегов проекта для выбора в задаче.
    /// </summary>
    /// <returns>Список тегов.</returns>
    Task<IEnumerable<ProjectTagEntity>> GetProjectTagsAsync();

    /// <summary>
    /// Метод получает список статусов задачи для выбора.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список статусов.</returns>
    Task<IEnumerable<TaskStatusOutput>> GetTaskStatusesAsync(long projectId, string account);

    /// <summary>
    /// Метод получает пользователей, которые могут быть выбраны в качестве исполнителя задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список пользователей.</returns>
    Task<IEnumerable<ProfileInfoEntity>> GetSelectTaskExecutorsAsync(long projectId, string account);

    /// <summary>
    /// Метод создает метку (тег) для задач пользователя.
    /// </summary>
    /// <param name="tagName">Название метки (тега).</param>
    /// <param name="tagDescription">Описание метки (тега).</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task CreateProjectTagAsync(string tagName, string tagDescription, long projectId, string account);

    /// <summary>
    /// Метод получает список статусов для выбора для создания нового статуса.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="templateId">Id шаблона.</param>
    /// <returns>Список статусов.</returns>
    Task<IEnumerable<ProjectManagmentTaskStatusTemplateEntity>> GetSelectableTaskStatusesAsync(long projectId,
        int templateId);

    /// <summary>
    /// Метод создает новый статус шаблона пользователя учитывая ассоциацию статуса.
    /// </summary>
    /// <param name="associationStatusSysName">Название статуса ассоциации.</param>
    /// <param name="statusName">Название статуса.</param>
    /// <param name="statusDescription">Описание статуса.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task CreateUserTaskStatusTemplateAsync(string associationStatusSysName, string statusName,
        string statusDescription, long projectId, string account);

    /// <summary>
    /// Метод получает доступные переходы в статусы задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="taskDetailType">Тип детализации.</param>
    /// <returns>Список доступных переходов.</returns>
    Task<IEnumerable<AvailableTaskStatusTransitionOutput>> GetAvailableTaskStatusTransitionsAsync(long projectId,
        string projectTaskId, string taskDetailType);

    /// <summary>
    /// Метод изменяет статус задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="changeStatusId">Id статуса задачи, на который нужно изменить.</param>
    /// <param name="taskId">Id задачи (здесь имеется в виду Id задачи в рамках проекта).</param>
    /// <param name="taskDetailType">Тип детализации.</param>
    /// <param name="token">Токен.</param>
    Task ChangeTaskStatusAsync(long projectId, string changeStatusId, string taskId, string taskDetailType,
        string token);
    
    /// <summary>
    /// Метод обновления описание задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи (здесь имеется в виду Id задачи в рамках проекта).</param>
    /// <param name="changedTaskDetails">Новое описание задачи.</param>
    /// <param name="account">Аккаунт.</param>
    Task UpdateTaskDetailsAsync(long projectId, string taskId, string changedTaskDetails, string account);
    
    /// <summary>
    /// Метод обновления названия задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи (здесь имеется в виду Id задачи в рамках проекта).</param>
    /// <param name="changedTaskDetails">Новое название задачи.</param>
    /// <param name="account">Аккаунт.</param>
    Task UpdateTaskNameAsync(long projectId, string taskId, string changedTaskName, string account);

    /// <summary>
    /// Метод привязывает тег к задаче проекта.
    /// Выбор происходит из набора тегов проекта.
    /// </summary>
    /// <param name="tagId">Id тега, который нужно привязать к задаче.</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task AttachTaskTagAsync(int tagId, string projectTaskId, long projectId, string account);
    
    /// <summary>
    /// Метод отвязывает тег от задачи проекта.
    /// </summary>
    /// <param name="tagId">Id тега, который нужно привязать к задаче.</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task DetachTaskTagAsync(int tagId, string projectTaskId, long projectId, string account);

    /// <summary>
    /// Метод привязывает наблюдателя задачи.
    /// </summary>
    /// <param name="watcherId">Id наблюдателя, которого нужно добавить к задаче..</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task AttachTaskWatcherAsync(long watcherId, string projectTaskId, long projectId, string account);
    
    /// <summary>
    /// Метод отвязывает наблюдателя задачи.
    /// </summary>
    /// <param name="watcherId">Id наблюдателя, которого нужно добавить к задаче..</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task DetachTaskWatcherAsync(long watcherId, string projectTaskId, long projectId, string account);

    /// <summary>
    /// Метод обновляет исполнителя задачи.
    /// </summary>
    /// <param name="executorId">Id нового исполнителя задачи.</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task UpdateTaskExecutorAsync(long executorId, string projectTaskId, long projectId, string account);

    /// <summary>
    /// Метод обновляет приоритет задачи.
    /// </summary>
    /// <param name="priorityId">Id приоритета.</param>
    /// <param name="taskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task UpdateTaskPriorityAsync(int priorityId, string projectTaskId, long projectId, string account);

    /// <summary>
    /// Метод создает связь с задачей (в зависимости от типа связи, который передали).
    /// </summary>
    /// <param name="taskLinkInput">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    Task CreateTaskLinkAsync(TaskLinkInput taskLinkInput, string account);

    /// <summary>
    /// Метод получает связи задачи (обычные связи).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список связей задачи.</returns>
    Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkDefaultAsync(long projectId, string projectTaskId,
        LinkTypeEnum linkType);

    /// <summary>
    /// Метод получает задачи проекта, которые доступны для создания связи с текущей задачей (разных типов связей).
    /// Под текущей задачей понимается задача, которую просматривает пользователь.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список задач, доступных к созданию связи.</returns>
    Task<IEnumerable<AvailableTaskLinkOutput>> GetAvailableTaskLinkAsync(long projectId, LinkTypeEnum linkType);
    
    /// <summary>
    /// Метод получает связи задачи (родительские связи).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список связей задачи.</returns>
    Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkParentAsync(long projectId, string projectTaskId,
        LinkTypeEnum linkType);
    
    /// <summary>
    /// Метод получает связи задачи (дочерние связи).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список связей задачи.</returns>
    Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkChildAsync(long projectId, string projectTaskId,
        LinkTypeEnum linkType);
    
    /// <summary>
    /// Метод получает связи задачи (связи зависит от).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список связей задачи.</returns>
    Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkDependAsync(long projectId, string projectTaskId,
        LinkTypeEnum linkType);
    
    /// <summary>
    /// Метод получает связи задачи (связи блокирующие).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="linkType">Тип связи.</param>
    /// <returns>Список связей задачи.</returns>
    Task<IEnumerable<GetTaskLinkOutput>> GetTaskLinkBlockedAsync(long projectId, string projectTaskId,
        LinkTypeEnum linkType);

    /// <summary>
    /// Метод разрывает связь определенного типа между задачами.
    /// </summary>
    /// <param name="linkType">Тип связи.</param>
    /// <param name="removedLinkId">Id задачи, с которой разрывается связь (задача в рамках проекта).</param>
    /// <param name="currentTaskId">Id текущей задачи (задача в рамках проекта).</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task RemoveTaskLinkAsync(LinkTypeEnum linkType, string removedLinkId, string currentTaskId, long projectId,
        string account);
    
    /// <summary>
    /// Метод загружает файлы по SFTP на сервер.
    /// </summary>
    /// <param name="files">Файлы для отправки.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи.</param>
    Task UploadFilesAsync(IFormFileCollection files, string account, long projectId, string taskId);

    /// <summary>
    /// Метод скачивает файл с сервера по SFTP.
    /// </summary>
    /// <param name="documentId">Id документа.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <returns>Данные документа.</returns>
    Task<FileContentResult> DownloadFileAsync(long documentId, long projectId, string projectTaskId);

    /// <summary>
    /// Метод получает файлы.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <returns>Файлы задачи.</returns>
    Task<IEnumerable<ProjectDocumentEntity>> GetProjectTaskFilesAsync(long projectId, string projectTaskId,
        TaskDetailTypeEnum taskDetailType);

    /// <summary>
    /// Метод удаляет файл задачи.
    /// </summary>
    /// <param name="documentId">Id документа.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    Task RemoveTaskFileAsync(long documentId, long projectId, string projectTaskId);

    /// <summary>
    /// Метод фиксирует выбранную пользователем стратегию представления.
    /// </summary>
    /// <param name="strategySysName">Системное название стратегии.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task FixationProjectViewStrategyAsync(string strategySysName, long projectId, string account);

    /// <summary>
    /// Метод создает комментарий задачи.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="comment">Комментарий задачи.</param>
    /// <param name="account">Аккаунт.</param>
    Task CreateTaskCommentAsync(string projectTaskId, long projectId, string comment, string account);

    /// <summary>
    /// Метод получает список комментариев задачи.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список комментариев задачи.</returns>
    Task<IEnumerable<TaskCommentOutput>> GetTaskCommentsAsync(string projectTaskId, long projectId);

    /// <summary>
    /// Метод обновляет комментарий задачи.
    /// </summary>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="commentId">Id комментария.</param>
    /// <param name="comment">Новый комментарий.</param>
    /// <param name="account">Аккаунт.</param>
    Task UpdateTaskCommentAsync(string projectTaskId, long projectId, long commentId, string comment, string account);

    /// <summary>
    /// Метод удаляет комментарий задачи.
    /// </summary>
    /// <param name="commentId">Id комментария для удаления.</param>
    /// <param name="account">Аккаунт.</param>
    Task DeleteTaskCommentAsync(long commentId, string account);
    
    /// <summary>
    /// Метод получает изображение аватара пользователя.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные файла.</returns>
    Task<FileContentResult> GetUserAvatarFileAsync(long projectId, string account);

    /// <summary>
    /// Метод загружает файл изображения аватара пользователя по SFTP на сервер.
    /// </summary>
    /// <param name="files">Файлы для отправки.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="projectId">Id проекта.</param>
    Task UploadUserAvatarFileAsync(IFormFileCollection files, string account, long projectId);

    /// <summary>
    /// Метод получает список эпиков.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список эпиков.</returns>
    Task<IEnumerable<EpicEntity>> GetEpicsAsync(long projectId);

    /// <summary>
    /// Метод получает список задач, историй для бэклога.
    /// Исключаются задачи в статусах: В архиве, готово, решена и тд.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список задач для бэклога.</returns>
    Task<ProjectManagmentWorkspaceResult> GetBacklogTasksAsync(long projectId, string account);

    /// <summary>
    /// Метод получает эпики, доступные к добавлению в них задачи.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Список эпиков.</returns>
    Task<IEnumerable<EpicEntity>> GetAvailableEpicsAsync(long projectId);

    /// <summary>
    /// Метод добавляет задачу в эпик.
    /// </summary>
    /// <param name="epicId">Id эпика.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>
    Task IncludeTaskEpicAsync(long epicId, IEnumerable<string> projectTaskIds, string account, string token);

    /// <summary>
    /// Метод получает список статусов истории для выбора.
    /// </summary>
    /// <returns>Список статусов истории.</returns>
    Task<IEnumerable<UserStoryStatusEntity>> GetUserStoryStatusesAsync();

    /// <summary>
    /// Метод планирует спринт.
    /// Добавляет задачи в спринт, если их указали при планировании спринта.
    /// </summary>
    /// <param name="planingSprintInput">Входная модель.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен.</param>
    Task PlaningSprintAsync(PlaningSprintInput planingSprintInput, string account, string token);

    /// <summary>
    /// Метод получает задачи эпика.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="epicId">Id эпика.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список задач эпика.</returns>
    Task<EpicTaskResult> GetEpicTasksAsync(long projectId, long epicId, string account);

    /// <summary>
    /// Метод получает название спринта, в который входит задача.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <returns>Данные спринта.</returns>
    Task<TaskSprintOutput> GetSprintTaskAsync(long projectId, string projectTaskId);

    /// <summary>
    /// Метод получает спринты, в которые может быть добавлена задача.
    /// Исключается спринт, в который задача уже добавлена.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <returns>Список спринтов, в которые может быть добавлена задача.</returns>
    Task<IEnumerable<TaskSprintOutput>> GetAvailableProjectSprintsAsync(long projectId, string projectTaskId);

    /// <summary>
    /// Метод добавляет/обновляет спринт, в который входит задача.
    /// </summary>
    /// <param name="sprintId">Id спринта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    Task InsertOrUpdateTaskSprintAsync(long sprintId, string projectTaskId);

    /// <summary>
    /// Метод получает все раб.пространства, в которых есть текущий пользователь.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список раб.пространств.</returns>
    Task<IEnumerable<WorkSpaceOutput>> GetWorkSpacesAsync(string account);

    /// <summary>
    /// Метод удаляет задачу проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectTaskId">Id задачи в рамках проекта.</param>
    /// <param name="account">Аккаунт.</param>
    Task RemoveProjectTaskAsync(long projectId, string projectTaskId, string account);
}