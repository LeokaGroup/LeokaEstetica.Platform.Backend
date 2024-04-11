using LeokaEstetica.Platform.Models.Dto.Input.Vacancy;
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
    /// <param name="vacancyInput">Входная модель.</param>
    /// <returns>Данные вакансии.</returns>
    Task<UserVacancyEntity> CreateVacancyAsync(VacancyInput vacancyInput);

    /// <summary>
    /// Метод обновляет вакансию.
    /// </summary>
    /// <param name="vacancyInput">Входная модель.</param>
    /// <returns>Данные вакансии.</returns>
    Task<UserVacancyEntity> UpdateVacancyAsync(VacancyInput vacancyInput);

    /// <summary>
    /// TODO: userId возможно нужкн будет использовать, если будет монетизация в каталоге вакансий. Если доступ будет только у тех пользователей, которые приобрели подписку.
    /// Метод получает список вакансий для каталога.
    /// </summary>
    /// <returns>Список вакансий.</returns>
    Task<List<CatalogVacancyOutput>> CatalogVacanciesAsync();
    
    /// <summary>
    /// Метод получает список вакансий для каталога (без выгрузки в память).
    /// </summary>
    /// <returns>Список вакансий.</returns>
    Task<IOrderedQueryable<CatalogVacancyOutput>> CatalogVacanciesWithoutMemoryAsync();

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
    /// <returns>Данные вакансии.</returns>
    Task<UserVacancyEntity> GetVacancyByVacancyIdAsync(long vacancyId);
    
    /// <summary>
    /// Метод находит Id владельца вакансии.
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
    /// <returns>Признак удаления и название вакансии.</returns>
    Task<(bool Success, string VacancyName)> DeleteVacancyAsync(long vacancyId, long userId);

    /// <summary>
    /// Метод првоеряет владельца вакансии.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак является ли пользователь владельцем вакансии.</returns>
    Task<bool> CheckVacancyOwnerAsync(long vacancyId, long userId);

    /// <summary>
    /// Метод получает список вакансий пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список вакансий.</returns>
    Task<IEnumerable<UserVacancyEntity>> GetUserVacanciesAsync(long userId);

    /// <summary>
    /// Метод Добавляет вакансию в архив.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    Task AddVacancyArchiveAsync(long vacancyId, long userId);
    
    /// <summary>
    /// Метод находит название вакансии по ее Id.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Название вакансии.</returns>
    Task<string> GetVacancyNameByIdAsync(long vacancyId);
    
    /// <summary>
    /// Метод проверяет, находится ли такая вакансия в архиве.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <returns>Признак проверки.</returns>
    Task<bool> CheckVacancyArchiveAsync(long vacancyId);
    
    /// <summary>
    /// Метод удаляет из архива вакансию.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="userId">Id пользователя.</param>
    Task<bool> DeleteVacancyArchiveAsync(long vacancyId, long userId);
    
    /// <summary>
    /// Метод получает список вакансий пользователя из архива.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список архивированных вакансий.</returns>
    Task<IEnumerable<ArchivedVacancyEntity>> GetUserVacanciesArchiveAsync(long userId);
    
    /// <summary>
    /// Метод получает кол-во вакансий пользователя в каталоге.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Кол-во вакансий в каталоге.</returns>
    Task<long> GetUserVacanciesCatalogCountAsync(long userId);
}