using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Models.Dto.Output.FareRule;
using LeokaEstetica.Platform.Models.Entities.FareRule;
using Microsoft.EntityFrameworkCore;

namespace LeokaEstetica.Platform.Database.Repositories.FareRule;

/// <summary>
/// Класс реализует методы репозитория правил тарифа.
/// </summary>
internal sealed class FareRuleRepository : BaseRepository, IFareRuleRepository
{
    // TODO: Выпилить датаконтекст и конфигурацию отсюда.
    private readonly PgContext _pgContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    /// <param name="connectionProvider">Провайдер БД.</param>
    public FareRuleRepository(PgContext pgContext,
        IConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
        _pgContext = pgContext;
    }

    /// <summary>
    /// Метод получает список тарифов.
    /// </summary>
    /// <returns>Список тарифов.</returns>
    public async Task<(IEnumerable<FareRuleCompositeOutput>? FareRules, IEnumerable<FareRuleAttributeOutput>?
        FareRuleAttributes, IEnumerable<FareRuleAttributeValueOutput>? FareRuleAttributeValues)> GetFareRulesAsync()
    {
        (IEnumerable<FareRuleCompositeOutput>? FareRules, IEnumerable<FareRuleAttributeOutput>? FareRuleAttributes,
            IEnumerable<FareRuleAttributeValueOutput>? FareRuleAttributeValues) result = (null, null, null);

        using var connection = await ConnectionProvider.GetConnectionAsync();

        var fareRulesQuery = "SELECT rule_id, " +
                             "rule_name, " +
                             "is_free, " +
                             "public_id " +
                             "FROM rules.fare_rules " +
                             "ORDER BY position";
        
        result.FareRules = await connection.QueryAsync<FareRuleCompositeOutput>(fareRulesQuery);

        var fareRuleAttributesQuery = "SELECT attribute_id, " +
                                      "attribute_name, " +
                                      "attribute_details " +
                                      "FROM rules.fare_rule_attributes " +
                                      "ORDER BY position";
                                      
        result.FareRuleAttributes = await connection.QueryAsync<FareRuleAttributeOutput>(fareRuleAttributesQuery);

        var fareRuleAttributeValuesQuery = "SELECT value_id, " +
                                           "rule_id, " +
                                           "is_price, " +
                                           "attribute_id, " +
                                           "measure, " +
                                           "min_value, " +
                                           "max_value, " +
                                           "is_range, " +
                                           "content_tooltip, " +
                                           "content " +
                                           "FROM rules.fare_rule_attribute_values " +
                                           "ORDER BY position";

        result.FareRuleAttributeValues = await connection.QueryAsync<FareRuleAttributeValueOutput>(
            fareRuleAttributeValuesQuery);
        
        return result;
    }

    /// <summary>
    /// Метод получает тариф по его Id.
    /// </summary>
    /// <param name="fareRuleId">Id тарифа.</param>
    /// <returns>Данные тарифа.</returns>
    public async Task<FareRuleEntity> GetByIdAsync(long fareRuleId)
    {
        var result = await _pgContext.FareRules
            .Where(fr => fr.RuleId == fareRuleId)
            .Select(fr => new FareRuleEntity(fr.PublicId)
            {
                RuleId = fr.RuleId,
                Name = fr.Name,
                Label = fr.Label,
                Currency = fr.Currency,
                Price = fr.Price,
                FareRuleItems = _pgContext.FareRuleItems
                    .Where(fri => fri.RuleId == fr.RuleId)
                    .OrderBy(o => o.Position)
                    .ToList(),
                Position = fr.Position,
                IsPopular = fr.IsPopular
            })
            .FirstOrDefaultAsync();

        return result;
    }
    
    /// <summary>
    /// Метод получает список названий входящих в список Ids тарифов.
    /// </summary>
    /// <returns>Список тарифов.</returns>
    public async Task<List<FareRuleEntity>> GetFareRulesNamesByIdsAsync(IEnumerable<long> fareRuleIds)
    {
        var result = await _pgContext.FareRules
            .Where(fr => fareRuleIds.Contains(fr.RuleId))
            .Select(fr => new FareRuleEntity(fr.PublicId)
            {
                RuleId = fr.RuleId,
                Name = fr.Name,
                Position = fr.Position
            })
            .OrderBy(o => o.Position)
            .ToListAsync();

        return result;
    }

    /// <summary>
    /// Метод получает тариф по его PublicId.
    /// </summary>
    /// <param name="objectId">Id объекта (тарифа).</param>
    /// <returns>Данные тарифа.</returns>
    public async Task<FareRuleEntity> GetFareRuleDetailsByObjectIdAsync(int objectId)
    {
        var result = await _pgContext.FareRules
            .Where(fr => fr.RuleId == objectId)
            .Select(fr => new FareRuleEntity(fr.PublicId)
            {
                RuleId = fr.RuleId,
                Name = fr.Name,
                Label = fr.Label,
                Currency = fr.Currency,
                Price = fr.Price,
                FareRuleItems = _pgContext.FareRuleItems
                    .Where(fri => fri.RuleId == fr.RuleId)
                    .OrderBy(o => o.Position)
                    .ToList(),
                Position = fr.Position,
                IsPopular = fr.IsPopular
            })
            .FirstOrDefaultAsync();

        return result;
    }

    /// <inheritdoc />
    public async Task<FareRuleAttributeCompositeOutput?> GetFareRuleByPublicIdAsync(Guid publicId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@publicId", publicId);

        var query = @"SELECT av.min_value, av.max_value, av.rule_id, fr.rule_name 
                        FROM rules.fare_rule_attribute_values AS av
                         INNER JOIN rules.fare_rules AS fr
                            ON av.rule_id = fr.rule_id
                        WHERE av.rule_id = @ruleId
                          AND av.attribute_id = 4";

        var result = await connection.QueryFirstOrDefaultAsync<FareRuleAttributeCompositeOutput>(query, parameters);

        return result;
    }
}