using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Configs;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Moderation;
using LeokaEstetica.Platform.Models.Entities.Vacancy;

namespace LeokaEstetica.Platform.Services.Abstractions.Vacancy;

/// <summary>
/// Абстракция сервиса вакансий.
/// </summary>
public interface IVacancyService
{
    /// <summary>
    /// Метод получает список меню вакансий.
    /// </summary>
    /// <returns>Список меню.</returns>
    Task<VacancyMenuItemsResultOutput> VacanciesMenuItemsAsync();

    /// <summary>
    /// Метод создает вакансию.
    /// </summary>
    /// <param name="vacancyInput">Входная модель.</param>
    /// <returns>Данные созданной вакансии.</returns>
    Task<VacancyOutput> CreateVacancyAsync(VacancyInput vacancyInput);
    
    /// <summary>
    /// Метод обновляет вакансию.
    /// </summary>
    /// <param name="vacancyInput">Входная модель.</param>
    /// <returns>Данные созданной вакансии.</returns>
    Task<UserVacancyEntity> UpdateVacancyAsync(VacancyInput vacancyInput);

    /// <summary>
    /// Метод получает список вакансий для каталога.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    Task<CatalogVacancyResultOutput> CatalogVacanciesAsync();

    /// <summary>
    /// Метод получает названия полей для таблицы вакансий проектов пользователя.
    /// Все названия столбцов этой таблицы одинаковые у всех пользователей.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    Task<IEnumerable<ProjectVacancyColumnNameOutput>> ProjectUserVacanciesColumnsNamesAsync();

    /// <summary>
    /// Метод получает вакансию по ее Id.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Данные вакансии.</returns>
    Task<VacancyOutput> GetVacancyByVacancyIdAsync(long vacancyId, string account);

    /// <summary>
    /// Метод фильтрации вакансий в зависимости от параметров фильтров.
    /// </summary>
    /// <param name="filters">Фильтры.</param>
    /// <returns>Список вакансий после фильтрации.</returns>
    Task<CatalogVacancyResultOutput> FilterVacanciesAsync(FilterVacancyInput filters);

    /// <summary>
    /// Метод удаляет вакансию.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    Task DeleteVacancyAsync(long vacancyId, string account, string token);

    /// <summary>
    /// Метод получает список вакансий пользователя.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    Task<VacancyResultOutput> GetUserVacanciesAsync(string account);

    /// <summary>
    /// Метод добавляет вакансию в архив.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    Task AddVacancyArchiveAsync(long vacancyId, string account, string token);

    /// <summary>
    /// Метод получает список замечаний вакансии, если они есть.
    /// </summary>
    /// <param name="projectId">Id вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список замечаний вакансии.</returns>
    Task<IEnumerable<VacancyRemarkEntity>> GetVacancyRemarksAsync(long vacancyId, string account);
}