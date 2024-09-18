namespace LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;

/// <summary>
/// Абстракция сервиса компаний.
/// </summary>
public interface ICompanyService
{
    /// <summary>
    /// Метод создает компанию.
    /// </summary>
    /// <param name="companyName">Название компании.</param>
    /// <param name="account">Аккаунт пользователя.</param>
    Task CreateCompanyAsync(string? companyName, string? account);
}