namespace LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция сервиса спринтов.
/// </summary>
public interface ISprintService
{
    Task GetSprintsAsync(long projectId);
}