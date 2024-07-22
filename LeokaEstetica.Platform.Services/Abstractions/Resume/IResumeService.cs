using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using LeokaEstetica.Platform.Models.Entities.Moderation;

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
    /// Метод получает анкету пользователя по ее Id.
    /// </summary>
    /// <param name="resumeId">Id анкеты пользователя.</param>
    /// <returns>Данные анкеты.</returns>
    Task<UserInfoOutput> GetResumeAsync(long resumeId);
    
    /// <summary>
    /// Метод получает список замечаний анкеты, если они есть.
    /// </summary>
    /// <param name="account">Аккаунт пользователя.</param>
    /// <returns>Список замечаний анкеты.</returns>
    Task<IEnumerable<ResumeRemarkEntity>> GetResumeRemarksAsync(string account);

    /// <summary>
    /// Метод записывает коды пользователей.
    /// </summary>
    /// <param name="resumes">Список анкет пользователей.</param>
    /// <returns>Результирующий список.</returns>
    Task<IEnumerable<UserInfoOutput>> SetUserCodesAsync(List<UserInfoOutput> resumes);

    /// <summary>
    /// Метод проставляет флаги вакансиям пользователя в зависимости от его подписки.
    /// </summary>
    /// <param name="vacancies">Список вакансий каталога.</param>
    /// <returns>Список вакансий каталога с проставленными тегами.</returns>
    Task<IEnumerable<UserInfoOutput>> SetVacanciesTagsAsync(List<UserInfoOutput> vacancies);
}