using LeokaEstetica.Platform.Models.Dto.Output.Resume;

namespace LeokaEstetica.Platform.Access.Abstractions.Resume;

/// <summary>
/// Абстракция сервиса проверки доступа к базе резюме.
/// </summary>
public interface IAccessResumeService
{
    /// <summary>
    /// Метод проверяет доступ пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Число, определяющее уровень доступа.</returns>
    Task<AcessResumeOutput> CheckAvailableResumesAsync(string account);
}