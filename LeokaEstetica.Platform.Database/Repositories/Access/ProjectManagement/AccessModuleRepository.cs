using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Access.ProjectManagement;
using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Database.Repositories.Access.ProjectManagement;

/// <summary>
/// Класс реализует методы репозитория проверки доступа к разным модулям платформы.
/// </summary>
internal sealed class AccessModuleRepository : BaseRepository, IAccessModuleRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider">Провайдер БД.</param>
    public AccessModuleRepository(IConnectionProvider connectionProvider) : base(connectionProvider)
    {
    }
    
    /// <inheritdoc />
    public async Task<bool> CheckAccessProjectManagementModuleOrComponentAsync(long projectId,
        AccessModuleTypeEnum accessModule, AccessModuleComponentTypeEnum accessModuleComponentType)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@projectId", projectId);
        parameters.Add("@accessModule", accessModule);
        parameters.Add("@accessModuleComponentType", accessModuleComponentType);

        var query = "SELECT amc.is_access " +
                    "FROM access.access_module AS am " +
                    "INNER JOIN access.access_module_components AS amc " +
                    "ON am.module_id = amc.module_id " +
                    "WHERE am.module_type = @accessModule " +
                    "AND amc.object_id = @projectId " +
                    "AND am.is_access " +
                    "AND amc.component_type = @accessModuleComponentType";
        
        var result = await connection.QueryFirstOrDefaultAsync<bool>(query, parameters);

        return result;
    }
}