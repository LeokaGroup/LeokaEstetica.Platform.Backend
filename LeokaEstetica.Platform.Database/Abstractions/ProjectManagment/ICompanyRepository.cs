using LeokaEstetica.Platform.Models.Dto.Communications.Output;
using LeokaEstetica.Platform.Models.Dto.ProjectManagement;

namespace LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция репозитория компаний.
/// </summary>
public interface ICompanyRepository
{
    /// <summary>
    /// Метод создает компанию.
    /// </summary>
    /// <param name="companyName">Название компании.</param>
    /// <param name="createdBy">Id пользователя создавшего компанию.</param>
    Task CreateCompanyAsync(string? companyName, long createdBy);

    /// <summary>
    /// Метод считает кол-во компаний пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Кол-во компаний пользователя.</returns>
    Task<int?> CalculateCountUserCompaniesByCompanyMemberIdAsync(long userId);

    /// <summary>
    /// Метод получает список компаний пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список компаний пользователя.</returns>
    Task<IEnumerable<CompanyOutput>?> GetUserCompaniesAsync(long userId);
    
    /// <summary>
    /// Метод получает объекты группы абстрактной области.
    /// </summary>
    /// <param name="abstractScopeId">Id абстрактной области.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Объекты группы абстрактной области.</returns>
    Task<IEnumerable<AbstractGroupOutput>> GetAbstractGroupObjectsAsync(long abstractScopeId, long userId);
}