﻿using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;

namespace LeokaEstetica.Platform.Database.Repositories.ProjectManagment;

/// <summary>
/// Класс реализует методы репозитория компаний.
/// </summary>
internal sealed class CompanyRepository : BaseRepository, ICompanyRepository
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="connectionProvider">Провайдер к БД.</param>
    public CompanyRepository(IConnectionProvider connectionProvider) : base(connectionProvider)
    {
    }

    /// <inheritdoc />
    public async Task CreateCompanyAsync(string? companyName, long createdBy)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@companyName", companyName);
        parameters.Add("@createdBy", createdBy);

        var query = "INSERT INTO project_management.organizations (organization_name, created_by) " +
                    "VALUES (@companyName, @createdBy)";

        await connection.ExecuteAsync(query, parameters);
    }

    /// <inheritdoc />
    public async Task<int?> CalculateCountUserCompaniesByCompanyMemberIdAsync(long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();
        
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);

        var query = "SELECT COUNT (organization_id) " +
                    "FROM project_management.organization_members " +
                    "WHERE member_id = @userId";

        var result = await connection.ExecuteScalarAsync<int?>(query, parameters);

        return result;
    }
}