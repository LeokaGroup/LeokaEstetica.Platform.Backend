using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Vacancy;
using LeokaEstetica.Platform.Models.Dto.Output.Vacancy;
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

    /// <summary>
    /// TODO: userId возможно нужкн будет использовать, если будет монетизация в каталоге вакансий. Если доступ будет только у тех пользователей, которые приобрели подписку.
    /// Метод получает список вакансий для каталога.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список вакансий.</returns>
    public async Task<List<CatalogVacancyOutput>> CatalogVacanciesAsync(long userId)
    {
        var result = await _pgContext.CatalogVacancies
            .Include(uv => uv.Vacancy)
            .Select(uv => new CatalogVacancyOutput
            {
                VacancyName = uv.Vacancy.VacancyName,
                DateCreated = uv.Vacancy.DateCreated,
                Employment = uv.Vacancy.Employment,
                Payment = uv.Vacancy.Payment,
                VacancyId = uv.Vacancy.VacancyId,
                VacancyText = uv.Vacancy.VacancyText,
                WorkExperience = uv.Vacancy.WorkExperience
            })
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод добавляет статус вакансии.
    /// </summary>
    /// <param name="vacancyId">Id вакансии.</param>
    /// <param name="statusName">Название статуса.</param>
    /// <param name="statusSysName">Системное название статуса.</param>
    public async Task AddVacancyStatusAsync(long vacancyId, string statusName, string statusSysName)
    {
        var vacancyStatus = new VacancyStatusEntity
        {
            VacancyId = vacancyId,
            VacancyStatusName = statusName,
            VacancyStatusSysName = statusSysName
        };
        await _pgContext.VacancyStatuses.AddAsync(vacancyStatus);
        await _pgContext.SaveChangesAsync();
    }
}