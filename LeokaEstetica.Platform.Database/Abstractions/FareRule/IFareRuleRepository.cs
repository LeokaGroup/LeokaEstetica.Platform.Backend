using LeokaEstetica.Platform.Models.Dto.Output.FareRule;
using LeokaEstetica.Platform.Models.Entities.FareRule;

namespace LeokaEstetica.Platform.Database.Abstractions.FareRule;

/// <summary>
/// TODO: Выпилить ненужные методы.
/// Абстракция репозитория правил тарифа.
/// </summary>
public interface IFareRuleRepository
{
    /// <summary>
    /// Метод получает список тарифов.
    /// </summary>
    /// <returns>Список тарифов.</returns>
    Task<IEnumerable<FareRuleEntity>> GetFareRulesAsync();

    /// <summary>
    /// Метод получает тариф по его Id.
    /// </summary>
    /// <param name="fareRuleId">Id тарифа.</param>
    /// <returns>Данные тарифа.</returns>
    Task<FareRuleEntity> GetByIdAsync(long fareRuleId);

    /// <summary>
    /// Метод получает список названий входящих в список Ids тарифов.
    /// </summary>
    /// <returns>Список тарифов.</returns>
    Task<List<FareRuleEntity>> GetFareRulesNamesByIdsAsync(IEnumerable<long> fareRuleIds);
    
    /// <summary>
    /// TODO: Выпилить его, у нас есть новый метод на Dapper.
    /// Метод получает тариф по его PublicId.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <returns>Данные тарифа.</returns>
    Task<FareRuleEntity> GetByPublicIdAsync(Guid publicId);

    /// <summary>
    /// TODO: Выпилим.
    /// Метод получает тариф по его ObjectId.
    /// </summary>
    /// <param name="objectId">Id объекта (тарифа).</param>
    /// <returns>Данные тарифа.</returns>
    Task<FareRuleEntity> GetFareRuleDetailsByObjectIdAsync(int objectId);

    /// <summary>
    /// Метод получает тариф по его PublicId.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <returns>Данные тарифа.</returns>
    Task<FareRuleAttributeCompositeOutput?> GetFareRuleByPublicIdAsync(Guid publicId);

    /// <summary>
    /// Метод получает Id тарифа пользователя по Id пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Id тарифа пользователя.</returns>
    Task<int> GetUserFareRuleIdByUserIdAsync(long userId);
}