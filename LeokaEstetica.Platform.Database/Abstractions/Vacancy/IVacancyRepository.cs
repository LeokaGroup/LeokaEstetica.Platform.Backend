using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.Vacancy;

namespace LeokaEstetica.Platform.Database.Abstractions.Vacancy;

/// <summary>
/// Абстракция репозитория вакансий.
/// </summary>
public interface IVacancyRepository
{
    /// <summary>
    /// Метод получает список меню вакансий.
    /// </summary>
    /// <returns>Список меню.</returns>
    Task<VacancyMenuItemEntity> VacanciesMenuItemsAsync();

    /// <summary>
    /// Метод создает вакансию.
    /// </summary>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="workExperience">Опыт работы.</param>
    /// <param name="employment">Занятость у вакансии.</param>
    /// <param name="payment">Оплата у вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Данные вакансии.</returns>
    Task<UserVacancyEntity> CreateVacancyAsync(string vacancyName, string vacancyText, string workExperience,
        string employment, string payment, long userId);

    /// <summary>
    /// Метод обновляет вакансию.
    /// </summary>
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="workExperience">Опыт работы.</param>
    /// <param name="employment">Занятость у вакансии.</param>
    /// <param name="payment">Оплата у вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Данные вакансии.</returns>
    Task<UserVacancyEntity> UpdateVacancyAsync(string vacancyName, string vacancyText, string workExperience,
        string employment, string payment, long userId, long vacancyId);

    /// <summary>
    /// TODO: userId возможно нужкн будет использовать, если будет монетизация в каталоге вакансий. Если доступ будет только у тех пользователей, которые приобрели подписку.
    /// Метод получает список вакансий для каталога.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    Task<List<CatalogVacancyOutput>> CatalogVacanciesAsync();

    /// <summary>
    /// Метод добавляет статус вакансии.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="statusName">Название статуса.</param>
    /// <param name="statusSysName">Системное название статуса.</param>
    Task AddVacancyStatusAsync(long vacancyId, string statusName, string statusSysName);

    /// <summary>
    /// Метод получает названия полей для таблицы вакансий проектов пользователя.
    /// Все названия столбцов этой таблицы одинаковые у всех пользователей.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    Task<IEnumerable<ProjectVacancyColumnNameEntity>> ProjectUserVacanciesColumnsNamesAsync();

    /// <summary>
    /// Метод получает вакансию по ее Id.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Данные вакансии.</returns>
    Task<UserVacancyEntity> GetVacancyByVacancyIdAsync(long vacancyId, long userId);
    
    /// <summary>
    /// Метод находит Id владельца проекта.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Id владельца вакансии.</returns>
    Task<long> GetVacancyOwnerIdAsync(long vacancyId);

    /// <summary>
    /// Метод получает название вакансии по ее Id.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Название вакансии.</returns>
    Task<string> GetVacancyNameByVacancyIdAsync(long vacancyId);
    
    /// <summary>
    /// Метод получает список вакансий для дальнейшей фильтрации.
    /// </summary>
    /// <returns>Список вакансий без выгрузки в память,
    /// так как этот список будем еще фильтровать.</returns>
    Task<IOrderedQueryable<CatalogVacancyOutput>> GetFiltersVacanciesAsync();

    /// <summary>
    /// Метод удаляет вакансию.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак удаления.</returns>
    Task<bool> DeleteVacancyAsync(long vacancyId, long userId);

    /// <summary>
    /// Метод првоеряет владельца вакансии.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак является ли пользователь владельцем вакансии.</returns>
    Task<bool> CheckProjectOwnerAsync(long vacancyId, long userId);

    /// <summary>
    /// Метод получает список вакансий пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список вакансий.</returns>
    Task<IEnumerable<UserVacancyEntity>> GetUserVacanciesAsync(long userId);
}