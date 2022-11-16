using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;

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
    /// <param name="vacancyName">Название вакансии.</param>
    /// <param name="vacancyText">Описание вакансии.</param>
    /// <param name="workExperience">Опыт работы.</param>
    /// <param name="employment">Занятость у вакансии.</param>
    /// <param name="payment">Оплата у вакансии.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Данные созданной вакансии.</returns>
    Task<CreateVacancyOutput> CreateVacancyAsync(string vacancyName, string vacancyText, string workExperience, string employment, string payment, string account);

    /// <summary>
    /// Метод получает список вакансий для каталога.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список вакансий.</returns>
    Task<CatalogVacancyResultOutput> CatalogVacanciesAsync(string account);
}