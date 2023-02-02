namespace LeokaEstetica.Platform.Access.Abstractions.Resume;

/// <summary>
/// Абстракция сервиса проверки доступа к базе резюме.
/// </summary>
public interface IAccessResumeService
{
    /// <summary>
    /// Метод проверяет доступ пользователя.
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    Task<int> CheckAvailableResumesAsync(string account);
}