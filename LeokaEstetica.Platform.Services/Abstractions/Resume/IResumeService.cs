using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using LeokaEstetica.Platform.Models.Entities.Moderation;
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
    Task<ResumeResultOutput> GetProfileInfosAsync();

    /// <summary>
    /// Метод получает анкету пользователя.
    /// </summary>
    /// <param name="resumeId">Id анкеты пользователя.</param>
    /// <returns>Данные анкеты.</returns>
    Task<ProfileInfoEntity> GetResumeAsync(long resumeId);
    
    /// <summary>
    /// Метод получает список замечаний анкеты, если они есть.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список замечаний анкеты.</returns>
    Task<IEnumerable<ResumeRemarkEntity>> GetResumeRemarksAsync(string account);
}