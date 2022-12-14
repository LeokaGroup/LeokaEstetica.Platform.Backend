using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.ProjectTeam;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Database.Abstractions.Project;

/// <summary>
/// Абстракция репозитория пользователя.
/// </summary>
public interface IProjectRepository
{
    /// <summary>
    /// Метод создает новый проект пользователя.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="statusSysName">Системное название статуса.</param>
    /// <param name="statusName">Русское название статуса.</param>
    /// <param name="projectStage">Стадия проекта.</param>
    /// <returns>Данные нового проекта.</returns>
    Task<UserProjectEntity> CreateProjectAsync(string projectName, string projectDetails, long userId,
        string statusSysName, string statusName, ProjectStageEnum projectStage);

    /// <summary>
    /// Метод получает названия полей для таблицы проектов пользователя.
    /// Все названия столбцов этой таблицы одинаковые у всех пользователей.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    Task<IEnumerable<ProjectColumnNameEntity>> UserProjectsColumnsNamesAsync();

    /// <summary>
    /// Метод проверяет, создан ли уже такой заказ под текущим пользователем с таким названием.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Создал либо нет.</returns>
    Task<bool> CheckCreatedProjectByProjectNameAsync(string projectName, long userId);

    /// <summary>
    /// Метод получает список проектов пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список проектов.</returns>
    Task<UserProjectResultOutput> UserProjectsAsync(long userId);

    /// <summary>
    /// TODO: Подумать, давать ли всем пользователям возможность просматривать каталог проектов или только тем, у кого есть подписка.
    /// Метод получает список проектов для каталога.
    /// </summary>
    /// <returns>Список проектов.</returns>
    Task<IEnumerable<CatalogProjectOutput>> CatalogProjectsAsync();

    /// <summary>
    /// Метод обновляет проект пользователя.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectStage">Стадия проекта.</param>
    /// <returns>Данные нового проекта.</returns>
    Task<UpdateProjectOutput> UpdateProjectAsync(string projectName, string projectDetails, long userId, long projectId,
        ProjectStageEnum projectStage);

    /// <summary>
    /// Метод получает проект для изменения или просмотра.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные проекта.</returns>
    Task<UserProjectEntity> GetProjectAsync(long projectId);

    /// <summary>
    /// Метод получает стадии проекта для выбора.
    /// </summary>
    /// <returns>Стадии проекта.</returns>
    Task<IEnumerable<ProjectStageEntity>> ProjectStagesAsync();

    /// <summary>
    /// Метод получает список вакансий проекта. Список вакансий, которые принадлежат владельцу проекта.
    /// </summary>
    /// <param name="projectId">Id проекта, вакансии которого нужно получить.</param>
    /// <returns>Список вакансий.</returns>
    Task<IEnumerable<ProjectVacancyEntity>> ProjectVacanciesAsync(long projectId);

    /// <summary>
    /// Метод прикрепляет вакансию к проекту.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Флаг успеха.</returns>
    Task<bool> AttachProjectVacancyAsync(long projectId, long vacancyId);

    /// <summary>
    /// Метод получает список вакансий проекта, которые можно прикрепить к проекту.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список вакансий проекта.</returns>
    Task<IEnumerable<ProjectVacancyEntity>> ProjectVacanciesAvailableAttachAsync(long projectId, long userId);

    /// <summary>
    /// Метод записывает отклик на проект.
    /// Отклик может быть с указанием вакансии, на которую идет отклик (если указана VacancyId).
    /// Отклик может быть без указаниея вакансии, на которую идет отклик (если не указана VacancyId).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Выходная модель с записанным откликом.</returns>
    Task<ProjectResponseEntity> WriteProjectResponseAsync(long projectId, long? vacancyId, long userId);

    /// <summary>
    /// Метод находит Id владельца проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Id владельца проекта.</returns>
    Task<long> GetProjectOwnerIdAsync(long projectId);

    /// <summary>
    /// Метод получает данные команды проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные команды проекта.</returns>
    Task<ProjectTeamEntity> GetProjectTeamAsync(long projectId);

    /// <summary>
    /// Метод получает список участников команды проекта по Id команды.
    /// </summary>
    /// <param name="teamId">Id проекта.</param>
    /// <returns>Список участников команды проекта.</returns>
    Task<List<ProjectTeamMemberEntity>> GetProjectTeamMembersAsync(long teamId);

    /// <summary>
    /// Метод получает названия полей для таблицы команды проекта пользователя.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    Task<IEnumerable<ProjectTeamColumnNameEntity>> ProjectTeamColumnsNamesAsync();

    /// <summary>
    /// Метод добавляет пользователя в команду проекта.
    /// </summary>
    /// <param name="userId">Id пользователя, который будет добавлен в команду проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="teamId">Id команды проекта.</param>
    /// <returns>Данные добавленного пользователя.</returns>
    Task<ProjectTeamMemberEntity> AddProjectTeamMemberAsync(long userId, long vacancyId, long teamId);

    /// <summary>
    /// Метод находит Id команды проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Id команды.</returns>
    Task<long> GetProjectTeamIdAsync(long projectId);

    /// <summary>
    /// Метод получает список проектов для дальнейшей фильтрации.
    /// </summary>
    /// <returns>Список проектов без выгрузки в память, так как этот список будем еще фильтровать.</returns>
    Task<IOrderedQueryable<CatalogProjectOutput>> GetFiltersProjectsAsync();
}