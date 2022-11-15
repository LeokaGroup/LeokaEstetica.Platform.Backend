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
    /// <returns>Данные созданной вакансии.</returns>
    Task<UserVacancyEntity> CreateVacancyAsync(string vacancyName, string vacancyText, string workExperience, string employment, string payment, long userId);
}