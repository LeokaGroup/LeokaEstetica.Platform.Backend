using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.Core.Data;
using LeokaEstetica.Platform.Database.Abstractions.FareRule;
using LeokaEstetica.Platform.Models.Dto.Output.FareRule;
using LeokaEstetica.Platform.Models.Entities.FareRule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LeokaEstetica.Platform.Database.Repositories.FareRule;

/// <summary>
/// Класс реализует методы репозитория правил тарифа.
/// </summary>
internal sealed class FareRuleRepository : BaseRepository, IFareRuleRepository
{
    // TODO: Выпилить датаконтекст и конфигурацию отсюда.
    private readonly PgContext _pgContext;
    private readonly IConfiguration _configuration;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="pgContext">Датаконтекст.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <param name="connectionProvider">Провайдер БД.</param>
    public FareRuleRepository(PgContext pgContext,
        IConfiguration configuration,
        IConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
        _pgContext = pgContext;
        _configuration = configuration;
    }

    /// <summary>
    /// Метод получает список тарифов.
    /// </summary>
    /// <returns>Список тарифов.</returns>
    public async Task<IEnumerable<FareRuleEntity>> GetFareRulesAsync()
    {
        var result = await _pgContext.FareRules
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
            .OrderBy(o => o.Position)
            .ToListAsync();

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
    /// TODO: Выпилить его, у нас есть новый метод на Dapper.
    /// Метод получает тариф по его PublicId.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <returns>Данные тарифа.</returns>
    public async Task<FareRuleEntity> GetByPublicIdAsync(Guid publicId)
    {
        try
        {
            var result = await _pgContext.FareRules
                .Where(fr => fr.PublicId == publicId)
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
        
        // TODO: При dispose PgContext пересоздаем датаконтекст и пробуем снова.
        catch (ObjectDisposedException _)
        {
            var pgContext = CreateNewPgContextFactory.CreateNewPgContext(_configuration);
            var result = await pgContext.FareRules
                .Where(fr => fr.PublicId == publicId)
                .Select(fr => new FareRuleEntity(fr.PublicId)
                {
                    RuleId = fr.RuleId,
                    Name = fr.Name,
                    Label = fr.Label,
                    Currency = fr.Currency,
                    Price = fr.Price,
                    FareRuleItems = pgContext.FareRuleItems
                        .Where(fri => fri.RuleId == fr.RuleId)
                        .OrderBy(o => o.Position)
                        .ToList(),
                    Position = fr.Position,
                    IsPopular = fr.IsPopular
                })
                .FirstOrDefaultAsync();
            
            return result;
        }
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
    public async Task<bool> CheckAvailableEmployeesCountFareRuleAsync(Guid publicId, int employeesCount)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@publicId", publicId);
        parameters.Add("@employeesCount", employeesCount);

        var query = @"SELECT CASE
           WHEN (SELECT max_value
                 FROM (SELECT av.max_value
                       FROM rules.fare_rule_attribute_values AS av
                                INNER JOIN rules.fare_rules AS fr
                                           ON av.rule_id = fr.rule_id
                       WHERE av.attribute_id = 2
                         AND fr.public_id = @publicId)) <= @employeesCount THEN TRUE
           ELSE FALSE END";

        var result = await connection.ExecuteScalarAsync<bool>(query, parameters);

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