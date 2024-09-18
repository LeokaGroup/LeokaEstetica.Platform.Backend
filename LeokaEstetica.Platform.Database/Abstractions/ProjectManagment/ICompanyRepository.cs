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
}