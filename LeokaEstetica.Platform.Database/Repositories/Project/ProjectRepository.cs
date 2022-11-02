using System.Data;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Project;
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
    public async Task<ProjectEntity> CreateProjectAsync(string projectName, string projectDetails, long userId)
    {
        var transaction = await _pgContext.Database
            .BeginTransactionAsync(IsolationLevel.ReadCommitted);
        
        try
        {
            var project = new ProjectEntity
            {
                ProjectName = projectName,
                ProjectDetails = projectDetails,
                UserId = userId
            };
            await _pgContext.Projects.AddAsync(project);
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
}