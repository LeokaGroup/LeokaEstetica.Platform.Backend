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
    
    /// <summary>
    /// Метод получает резюме для фильтрации без выгрузки в память.
    /// </summary>
    /// <returns>Резюме без выгрузки в память.</returns>
    Task<IOrderedQueryable<ProfileInfoEntity>> GetFilterResumesAsync();

    /// <summary>
    /// Метод получает анкету пользователя.
    /// </summary>
    /// <param name="resumeId">Id анкеты пользователя.</param>
    /// <returns>Данные анкеты.</returns>
    Task<ProfileInfoEntity> GetResumeAsync(long resumeId);
    
    /// <summary>
    /// Метод првоеряет владельца анкеты.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак является ли пользователь владельцем анкеты.</returns>
    Task<bool> CheckResumeOwnerAsync(long profileInfoId, long userId);
}