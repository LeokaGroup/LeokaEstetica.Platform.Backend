using LeokaEstetica.Platform.Models.Entities.Profile;

namespace LeokaEstetica.Platform.Services.Abstractions.Resume;

/// <summary>
/// Абстракция сервиса базы резюме.
/// </summary>
public interface IResumeService
{
    /// <summary>
    /// Метод получает список резюме.
    /// </summary>
    /// <returns>Список резюме.</returns>
    Task<IEnumerable<ProfileInfoEntity>> GetProfileInfosAsync();

    /// <summary>
    /// Метод получает анкету пользователя.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Данные анкеты.</returns>
    Task<ProfileInfoEntity> GetResumeAsync(string account);
}