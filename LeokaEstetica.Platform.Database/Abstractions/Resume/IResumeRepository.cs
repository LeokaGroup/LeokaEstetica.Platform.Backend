using LeokaEstetica.Platform.Models.Entities.Profile;

namespace LeokaEstetica.Platform.Database.Abstractions.Resume;

/// <summary>
/// Абстракция репозитория базы резюме.
/// </summary>
public interface IResumeRepository
{
    /// <summary>
    /// Метод получает список резюме.
    /// </summary>
    /// <returns>Список резюме.</returns>
    Task<List<ProfileInfoEntity>> GetProfileInfosAsync();
}