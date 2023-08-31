namespace LeokaEstetica.Platform.Database.Abstractions.Project;

/// <summary>
/// Абстракция репозитория откликов на проекты.
/// </summary>
public interface IProjectResponseRepository
{
    /// <summary>
    /// Метод связывает диалог с вакансией (если при отклике на проект, отклик был с указанием вакансии).
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    Task SetReferenceVacancyDialogAsync(long projectId, long userId);
}