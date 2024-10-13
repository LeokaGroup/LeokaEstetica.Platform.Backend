using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.Communications;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;

namespace LeokaEstetica.Platform.Database.Repositories.Communications;

/// <summary>
/// Класс реализует методы репозитория абстрактных областей чата.
/// </summary>
internal sealed class AbstractScopeRepository : BaseRepository, IAbstractScopeRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider">Провайдер к БД.</param>
    public AbstractScopeRepository(IConnectionProvider connectionProvider)
        : base(connectionProvider)
    {
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AbstractScopeOutput>> GetAbstractScopesAsync(long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);

        // При расширении абстрактных областей - добавлять новые путем UNION.
        var query = "SELECT DISTINCT (o.organization_id) AS abstract_scope_id, " +
                    "COALESCE(o.organization_name, 'Не задано название') AS label, " +
                    "'company'::communications.abstract_scope_type_enum AS abstract_scope_type," +
                    "om.member_id AS user_id " +
                    "FROM project_management.organizations AS o " +
                    "INNER JOIN project_management.organization_projects AS op " +
                    "ON o.organization_id = op.organization_id " +
                    "INNER JOIN project_management.organization_members AS om " +
                    "ON o.organization_id = om.organization_id " +
                    "WHERE om.member_id = @userId " +
                    "AND op.is_active " +
                    "GROUP BY abstract_scope_id, user_id, label " +
                    "ORDER BY abstract_scope_id";

        var result = await connection.QueryAsync<AbstractScopeOutput>(query, parameters);

        return result;
    }
}