using LeokaEstetica.Platform.Access.Enums;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
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
    /// <returns>Данные нового проекта.</returns>
    Task<UserProjectEntity> CreateProjectAsync(string projectName, string projectDetails, string account, ProjectStageEnum projectStage);

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
    Task<IEnumerable<CatalogProjectOutput>> CatalogProjectsAsync();

    /// <summary>
    /// Метод обновляет проект пользователя.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="projectStage">Стадия проекта.</param>
    /// <returns>Данные нового проекта.</returns>
    Task<UpdateProjectOutput> UpdateProjectAsync(string projectName, string projectDetails, string account, long projectId, ProjectStageEnum projectStage);

    /// <summary>
    /// Метод получает проект для изменения или просмотра.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="mode">Режим. Чтение или изменение.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные проекта.</returns>
    Task<UserProjectEntity> GetProjectAsync(long projectId, ModeEnum mode, string account);

    /// <summary>
    /// Метод получает стадии проекта для выбора.
    /// </summary>
    /// <returns>Стадии проекта.</returns>
    Task<IEnumerable<ProjectStageOutput>> ProjectStagesAsync();

    /// <summary>
    /// Метод получает список вакансий проекта. Список вакансий, которые принадлежат владельцу проекта.
    /// </summary>
    /// <param name="projectId">Id проекта, вакансии которого нужно получить.</param>
    /// <returns>Список вакансий.</returns>
    Task< IEnumerable<ProjectVacancyEntity>> ProjectVacanciesAsync(long projectId);

    /// <summary>
    /// Метод создает вакансию проекта. При этом автоматически происходит привязка к проекту.
    /// </summary>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="workExperience">Опыт работы.</param>
    /// <param name="employment">Занятость у вакансии.</param>
    /// <param name="payment">Оплата у вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Данные вакансии.</returns>
    Task<UserVacancyEntity> CreateProjectVacancyAsync(string vacancyName, string vacancyText, long projectId, string employment, string payment, string workExperience, string account);
}