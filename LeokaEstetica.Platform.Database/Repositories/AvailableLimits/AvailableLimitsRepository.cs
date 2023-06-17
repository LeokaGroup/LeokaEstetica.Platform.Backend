using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.AvailableLimits;
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
}