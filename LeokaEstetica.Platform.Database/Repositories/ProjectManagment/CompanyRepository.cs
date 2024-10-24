﻿using Dapper;
using LeokaEstetica.Platform.Base.Abstractions.Connection;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.Base;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;
using LeokaEstetica.Platform.Models.Dto.ProjectManagement;
using LeokaEstetica.Platform.Models.Enums;
using Enum = LeokaEstetica.Platform.Models.Enums.Enum;

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
    public CompanyRepository(IConnectionProvider connectionProvider)
        : base(connectionProvider)
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

    /// <inheritdoc />
    public async Task<IEnumerable<CompanyOutput>?> GetUserCompaniesAsync(long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);

        var query = "SELECT org.organization_id AS CompanyId, " +
                    "org.organization_name AS CompanyName, " +
                    "org.created_by " +
                    "FROM project_management.organization_members AS om " +
                    "INNER JOIN project_management.organizations AS org " +
                    "ON om.organization_id = org.organization_id " +
                    "WHERE om.member_id = @userId " +
                    "AND om.member_role = 'Владелец'";

        var result = await connection.QueryAsync<CompanyOutput>(query, parameters);

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AbstractGroupOutput>> GetCompanyProjectsAsync(long abstractScopeId, long userId)
    {
        using var connection = await ConnectionProvider.GetConnectionAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@companyId", abstractScopeId);
        parameters.Add("@userId", userId);
        parameters.Add("@dialogGroupType", new Enum(DialogGroupTypeEnum.Project));

        var query = "SELECT op.project_id AS abstract_group_id, " +
                    "COALESCE((SELECT \"ParamValue\" " +
                    "FROM \"Configs\".\"ProjectManagmentProjectSettings\" " +
                    "WHERE \"ProjectId\" = op.project_id " +
                    "AND \"ParamKey\" = 'ProjectManagement.ProjectName'), 'Проект без названия') " +
                    "AS label, " +
                    "'project'::communications.abstract_group_type_enum AS abstract_group_type," +
                    "(CASE WHEN (SELECT COUNT(message_id)) > 0 THEN TRUE ELSE FALSE END) AS has_dialogs " +
                    "FROM project_management.organization_members AS om " +
                    "INNER JOIN project_management.organization_projects AS op " +
                    "ON om.organization_id = op.organization_id " +
                    "LEFT JOIN communications.main_info_dialogs AS id " +
                    "ON op.project_id = id.abstract_scope_id " +
                    "LEFT JOIN communications.dialog_members AS dm " +
                    "ON id.dialog_id = dm.dialog_id " +
                    "LEFT JOIN communications.dialog_messages AS dmes " +
                    "ON id.dialog_id = dmes.dialog_id " +
                    "WHERE op.is_active " +
                    "AND om.member_id = @userId " +
                    "AND op.organization_id = @companyId " +
                    "AND dialog_group_type = @dialogGroupType " +
                    "GROUP BY abstract_group_id " +
                    "ORDER BY abstract_group_id";

        var result = await connection.QueryAsync<AbstractGroupOutput>(query, parameters);

        return result;
    }
}