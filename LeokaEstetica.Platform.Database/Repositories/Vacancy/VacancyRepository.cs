using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Vacancy;

/// <summary>
/// Класс реализует методы репозитория вакансий. 
/// </summary>
public sealed class VacancyRepository : IVacancyRepository
{
    private readonly PgContext _pgContext;
    
    public VacancyRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод получает список меню вакансий.
    /// </summary>
    /// <returns>Список меню.</returns>
    public async Task<VacancyMenuItemEntity> VacanciesMenuItemsAsync()
    {
        var result = await _pgContext.VacancyMenuItems
            .FirstOrDefaultAsync();

        return result;
    }

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
    public async Task<UserVacancyEntity> CreateVacancyAsync(string vacancyName, string vacancyText, string workExperience, string employment,
        string payment, long userId)
    {
        var vacancy = new UserVacancyEntity
        {
            DateCreated = DateTime.UtcNow,
            VacancyName = vacancyName,
            VacancyText = vacancyText,
            WorkExperience = workExperience,
            Employment = employment,
            Payment = payment,
            UserId = userId
        };
        await _pgContext.UserVacancies.AddAsync(vacancy);
        await _pgContext.SaveChangesAsync();

        return vacancy;
    }
}