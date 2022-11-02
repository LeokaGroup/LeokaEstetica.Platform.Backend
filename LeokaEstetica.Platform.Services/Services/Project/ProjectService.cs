using LeokaEstetica.Platform.Models.Dto.Output.Project;
using LeokaEstetica.Platform.Services.Abstractions.Project;

namespace LeokaEstetica.Platform.Services.Services.Project;

/// <summary>
/// Класс реализует методы сервиса проектов.
/// </summary>
public sealed class ProjectService : IProjectService
{
    public ProjectService()
    {
    }

    /// <summary>
    /// Метод создает новый проект пользователя.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Данные нового проекта.</returns>
    public Task<CreateProjectOutput> CreateProjectAsync(string projectName, string projectDetails, string account)
    {
        throw new NotImplementedException();
    }
}