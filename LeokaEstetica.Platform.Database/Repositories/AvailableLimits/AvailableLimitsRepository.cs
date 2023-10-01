using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.AvailableLimits;
using LeokaEstetica.Platform.Models.Entities.Project;
using LeokaEstetica.Platform.Models.Entities.Vacancy;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.AvailableLimits;

/// <summary>
/// Класс реализует методы репозитория проверки лимитов.
/// </summary>
internal sealed class AvailableLimitsRepository : IAvailableLimitsRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public AvailableLimitsRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает кол-во уже созданных проектов пользователем.
    /// Считаем только активные проекты (т.е. которые находятся в каталоге).
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Кол-во созданных пользователем проектов.</returns>
    public async Task<int> CheckAvailableCreateProjectAsync(long userId)
    {
        // Считаем кол-во именно в каталоге, потому что что ограничиваем лишь на активные. 
        var result = await _pgContext.CatalogProjects
            .Where(p => p.Project.UserId == userId)
            .CountAsync();

        return result;
    }

    /// <summary>
    /// Метод получает кол-во уже созданных вакансий пользователем.
    /// Считаем только активные вакансии (т.е. которые находятся в каталоге).
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Кол-во созданных пользователем вакансий.</returns>
    public async Task<int> CheckAvailableCreateVacancyAsync(long userId)
    {
        // Считаем кол-во именно в каталоге, потому что что ограничиваем лишь на активные.
        var result = await _pgContext.CatalogVacancies
            .Where(p => p.Vacancy.UserId == userId)
            .CountAsync();

        return result;
    }

    /// <summary>
    /// Метод добавляет в архив все проекты и вакансии пользователя, пока не пройдем по лимитам бесплатного тарифа.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    public async Task RestrictionFreeLimitsAsync(long userId)
    {
        var countProjectsCatalog = await _pgContext.CatalogProjects
            .CountAsync(p => p.Project.UserId == userId);
        
        if (countProjectsCatalog > 4)
        {
            // Получаем все проекты пользователя, чтобы ограничить до 4 проектов.
            var archivedProjects = await _pgContext.UserProjects
                .Where(p => p.UserId == userId)
                .Skip(4)
                .Select(x => new ArchivedProjectEntity
                {
                    DateArchived = DateTime.UtcNow,
                    ProjectId = x.ProjectId,
                    UserId = userId
                })
                .ToListAsync();
        
            await _pgContext.ArchivedProjects.AddRangeAsync(archivedProjects);
        }

        var countVacanciesCatalog = await _pgContext.CatalogVacancies
            .CountAsync(p => p.Vacancy.UserId == userId);

        if (countVacanciesCatalog > 5)
        {
            // Получаем все вакансии пользователя, чтобы ограничить до 5 вакансий.
            var archivedVacancies = await _pgContext.UserVacancies
                .Where(p => p.UserId == userId)
                .Skip(5)
                .Select(x => new ArchivedVacancyEntity
                {
                    DateArchived = DateTime.UtcNow,
                    VacancyId = x.VacancyId,
                    UserId = userId
                })
                .ToListAsync();
        
            await _pgContext.ArchivedVacancies.AddRangeAsync(archivedVacancies);   
        }
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}