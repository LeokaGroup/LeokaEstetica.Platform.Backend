using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Core.Enums;
using LeokaEstetica.Platform.Models.Dto.Input.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Output.ProjectTeam;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.ProjectTeam;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Services.Abstractions.Project;

/// <summary>
/// Абстракция сервиса работы с проектами.
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Метод создает новый проект пользователя.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="projectStage">Стадия проекта.</param>
    /// <param name="demands">Требования проекта.</param>
    /// <param name="conditions">Условия проекта.</param>
    /// <returns>Данные нового проекта.</returns>
    Task<UserProjectEntity> CreateProjectAsync(string projectName, string projectDetails, string account,
        ProjectStageEnum projectStage, string token, string demands, string conditions);

    /// <summary>
    /// Метод получает названия полей для таблицы проектов пользователя.
    /// Все названия столбцов этой таблицы одинаковые у всех пользователей.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    Task<IEnumerable<ProjectColumnNameOutput>> UserProjectsColumnsNamesAsync();

    /// <summary>
    /// Метод получает список проектов пользователя.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список проектов.</returns>
    Task<UserProjectResultOutput> UserProjectsAsync(string account);

    /// <summary>
    /// TODO: Подумать, давать ли всем пользователям возможность просматривать каталог проектов или только тем, у кого есть подписка.
    /// Метод получает список проектов для каталога.
    /// </summary>
    /// <returns>Список проектов.</returns>
    Task<CatalogProjectResultOutput> CatalogProjectsAsync();

    /// <summary>
    /// Метод обновляет проект пользователя.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectStage">Стадия проекта.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Данные нового проекта.</returns>
    Task<UpdateProjectOutput> UpdateProjectAsync(string projectName, string projectDetails, string account,
        long projectId, ProjectStageEnum projectStage, string token);

    /// <summary>
    /// Метод получает проект для изменения или просмотра.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="mode">Режим. Чтение или изменение.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные проекта.</returns>
    Task<ProjectOutput> GetProjectAsync(long projectId, ModeEnum mode, string account);

    /// <summary>
    /// Метод получает стадии проекта для выбора.
    /// </summary>
    /// <returns>Стадии проекта.</returns>
    Task<IEnumerable<ProjectStageOutput>> ProjectStagesAsync();

    /// <summary>
    /// Метод получает список вакансий проекта. Список вакансий, которые принадлежат владельцу проекта.
    /// </summary>
    /// <param name="projectId">Id проекта, вакансии которого нужно получить.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Список вакансий.</returns>
    Task<ProjectVacancyResultOutput> ProjectVacanciesAsync(long projectId, string account, string token);

    /// <summary>
    /// Метод создает вакансию проекта. При этом автоматически происходит привязка к проекту.
    /// </summary>
    /// <param name="createProjectVacancyInput">Входная модель.</param>
    /// <returns>Данные вакансии.</returns>
    Task<VacancyOutput> CreateProjectVacancyAsync(CreateProjectVacancyInput createProjectVacancyInput);

    /// <summary>
    /// Метод получает список вакансий проекта, которые могут быть прикреплены у проекту пользователя.
    /// </summary>
    /// <param name="projectId">Id проекта, для которого получить список вакансий.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список вакансий проекта.</returns>
    Task<IEnumerable<ProjectVacancyEntity>> ProjectVacanciesAvailableAttachAsync(long projectId, string account);

    /// <summary>
    /// Метод прикрепляет вакансию к проекту.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="token">Токен пользователя.</param>
    Task AttachProjectVacancyAsync(long projectId, long vacancyId, string account, string token);

    /// <summary>
    /// Метод записывает отклик на проект.
    /// Отклик может быть с указанием вакансии, на которую идет отклик (если указана VacancyId).
    /// Отклик может быть без указаниея вакансии, на которую идет отклик (если не указана VacancyId).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Выходная модель с записанным откликом.</returns>
    Task<ProjectResponseEntity> WriteProjectResponseAsync(long projectId, long? vacancyId, string account,
        string token);

    /// <summary>
    /// Метод получает команду проекта.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Данные команды проекта.</returns>
    Task<IEnumerable<ProjectTeamOutput>> GetProjectTeamAsync(long projectId);

    /// <summary>
    /// Метод получает названия полей для таблицы команды проекта пользователя.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    Task<IEnumerable<ProjectTeamColumnNameEntity>> ProjectTeamColumnsNamesAsync();

    /// <summary>
    /// Метод добавляет в команду проекта пользователей.
    /// </summary>
    /// <param name="inviteText">Текст, который будет использоваться для поиска пользователя для приглашения.</param>
    /// <param name="inviteType">Способ приглашения.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Добавленный пользователь.</returns>
    Task<ProjectTeamMemberEntity> InviteProjectTeamAsync(string inviteText, ProjectInviteTypeEnum inviteType,
        long projectId, long? vacancyId, string account, string token);

    /// <summary>
    /// Метод фильтрации проектов в зависимости от параметров фильтров.
    /// </summary>
    /// <param name="filterProjectInput">Входная модель.</param>
    /// <returns>Список проектов после фильтрации.</returns>
    Task<IEnumerable<CatalogProjectOutput>> FilterProjectsAsync(FilterProjectInput filterProjectInput);

    /// <summary>
    /// Метод удаляет вакансию проекта.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    Task DeleteProjectVacancyAsync(long vacancyId, long projectId, string account, string token);

    /// <summary>
    /// Метод удаляет проект и все, что с ним связано.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    Task DeleteProjectAsync(long projectId, string account, string token);

    /// <summary>
    /// Метод получает список вакансий доступных к отклику.
    /// Для владельца проекта будет возвращаться пустой список.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список вакансий доступных к отклику.</returns>
    Task<IEnumerable<ProjectVacancyEntity>> GetAvailableResponseProjectVacanciesAsync(long projectId, string account);

    /// <summary>
    /// Метод получает список проектов пользователя из архива.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список архивированных проектов.</returns>
    Task<UserProjectArchiveResultOutput> GetUserProjectsArchiveAsync(string account);

    /// <summary>
    /// Метод удаляет участника проекта из команды.
    /// </summary>
    /// <param name="projectId">Id проекта</param>
    /// <param name="userId">Id пользователя, которого будем удалять из команды</param>
    /// <param name="token">Токен.</param>
    Task DeleteProjectTeamMemberAsync(long projectId, long userId, string token);

    /// <summary>
    /// Метод покидания команды проекта.
    /// </summary>
    /// <param name="projectId">Id проекта</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="token">Токен.</param>
    Task LeaveProjectTeamAsync(long projectId, string account, string token);

    /// <summary>
    /// Метод получает список замечаний проекта, если они есть.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список замечаний проекта.</returns>
    Task<IEnumerable<ProjectRemarkEntity>> GetProjectRemarksAsync(long projectId, string account);
}