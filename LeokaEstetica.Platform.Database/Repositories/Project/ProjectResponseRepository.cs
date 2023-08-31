using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.Project;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.Project;

/// <summary>
/// Класс реализует методы репозитория откликов на проекты.
/// </summary>
internal sealed class ProjectResponseRepository : IProjectResponseRepository
{
    private readonly PgContext _pgContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    public ProjectResponseRepository(PgContext pgContext)
    {
        _pgContext = pgContext;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод связывает диалог с вакансией (если при отклике на проект, отклик был с указанием вакансии).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    public async Task SetReferenceVacancyDialogAsync(long projectId, long userId)
    {
        var response = await _pgContext.ProjectResponses
            .FirstOrDefaultAsync(r => r.ProjectId == projectId 
                                      && r.UserId == userId);

        if (response is null)
        {
            throw new InvalidOperationException("Не удалось получить отклик на проект. " +
                                                $"ProjectId: {projectId}. " +
                                                $"UserId: {userId}");
        }

        // Если отклик был без вакансии, то ничего не делаем.
        if (!response.VacancyId.HasValue)
        {
            return;
        }
        
        // Иначе связываем диалог с вакансией.
        var dialog = await (from dm in _pgContext.DialogMembers
                join d in _pgContext.Dialogs
                    on dm.DialogId
                    equals d.DialogId
                where dm.UserId == userId
                      && d.ProjectId == projectId
                select d)
            .FirstOrDefaultAsync();

        if (dialog is null)
        {
            throw new InvalidOperationException("Не удалось связать диалог с вакансией. " +
                                                $"ProjectId: {projectId}. " +
                                                $"UserId: {userId}");
        }

        dialog.VacancyId = response.VacancyId;
        await _pgContext.SaveChangesAsync();
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}