using LeokaEstetica.Platform.Models.Dto.Output.Resume;
using LeokaEstetica.Platform.Models.Entities.Profile;

namespace LeokaEstetica.Platform.Database.Abstractions.Resume;

/// <summary>
/// Абстракция репозитория базы резюме.
/// </summary>
public interface IResumeRepository
{

    /// <summary>
    /// Метод получает резюме для фильтрации без выгрузки в память.
    /// </summary>
    /// <returns>Резюме без выгрузки в память.</returns>
    Task<IOrderedQueryable<ProfileInfoEntity>> GetFilterResumesAsync();

    /// <summary>
    /// Метод получает анкету пользователя по ее Id.
    /// </summary>
    /// <param name="resumeId">Id анкеты пользователя.</param>
    /// <returns>Данные анкеты.</returns>
    Task<UserInfoOutput> GetResumeAsync(long resumeId);
    
    /// <summary>
    /// Метод получает анкеты пользователей по Id пользователей.
    /// </summary>
    /// <param name="usersIds">Id пользователей.</param>
    /// <returns>Список анкет.</returns>
    Task<IEnumerable<ProfileInfoEntity>> GetResumesAsync(IEnumerable<long> usersIds);
    
    /// <summary>
    /// Метод првоеряет владельца анкеты.
    /// </summary>
    /// <param name="profileInfoId">Id анкеты.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Признак является ли пользователь владельцем анкеты.</returns>
    Task<bool> CheckResumeOwnerAsync(long profileInfoId, long userId);
    
    /// <summary>
    /// Метод получает резюме для каталога анкет применяя пагинацию.
    /// </summary>
    /// <param name="count">Кол-во записей, которые нужно брать.</param>
    /// <param name="lastId">Id последней записи на странице фронта.
    /// Используется для пагинации. Если не передали, значит выдаем первые n-записей.</param>
    /// <returns>Список анкет.</returns>
    Task<PaginationResumeOutput?> GetPaginationResumesAsync(long count, long? lastId);
}