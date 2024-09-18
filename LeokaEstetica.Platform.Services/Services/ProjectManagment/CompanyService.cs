using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.ProjectManagment;
using LeokaEstetica.Platform.Services.Abstractions.ProjectManagment;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Services.Services.ProjectManagment;

/// <summary>
/// Класс реализует методы сервиса компаний.
/// </summary>
internal sealed class CompanyService : ICompanyService
{
    private readonly ILogger<CompanyService> _logger;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="companyRepository">Репозиторий компаний.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    public CompanyService(ILogger<CompanyService> logger,
        ICompanyRepository companyRepository,
        IUserRepository userRepository)
    {
        _logger = logger;
        _companyRepository = companyRepository;
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task CreateCompanyAsync(string? companyName, string? account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account!);

            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account!);
                throw ex;
            }

            await _companyRepository.CreateCompanyAsync(companyName, userId);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}