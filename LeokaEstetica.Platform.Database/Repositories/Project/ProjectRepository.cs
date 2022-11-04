using System.Data;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using LeokaEstetica.Platform.Models.Entities.Configs;
using LeokaEstetica.Platform.Models.Entities.Project;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Project;

/// <summary>
/// Класс реализует метод репозитория проектов.
/// </summary>
public sealed class ProjectRepository : IProjectRepository
{
    private readonly PgContext _pgContext;
    
    public ProjectRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод создает новый проект пользователя.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Данные нового проекта.</returns>
    public async Task<UserProjectEntity> CreateProjectAsync(string projectName, string projectDetails, long userId)
    {
        var transaction = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);
        
        try
        {
            var project = new UserProjectEntity
            {
                ProjectName = projectName,
                ProjectDetails = projectDetails,
                UserId = userId,
                ProjectCode = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow
            };
            await _pgContext.UserProjects.AddAsync(project);
            await _pgContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return project;
        }

        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Метод получает названия полей для таблицы проектов пользователя.
    /// Все названия столбцов этой таблицы одинаковые у всех пользователей.
    /// </summary>
    /// <returns>Список названий полей таблицы.</returns>
    public async Task<IEnumerable<ColumnNameEntity>> UserProjectsColumnsNamesAsync()
    {
        var result = await _pgContext.ColumnsNames
            .OrderBy(o => o.Position)
            .ToListAsync();

        return result;
    }
}