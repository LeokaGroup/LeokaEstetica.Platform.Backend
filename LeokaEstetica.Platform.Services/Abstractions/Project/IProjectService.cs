using LeokaEstetica.Platform.Models.Dto.Output.Project;

namespace LeokaEstetica.Platform.Services.Abstractions.Project;

/// <summary>
/// Абстракция сервиса работы с проектами.
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Метод создает новый проект пользователя.
    /// </summary>
    /// <param name="projectName">Название проекта.</param>
    /// <param name="projectDetails">Описание проекта.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Данные нового проекта.</returns>
    Task<CreateProjectOutput> CreateProjectAsync(string projectName, string projectDetails, string account);
}