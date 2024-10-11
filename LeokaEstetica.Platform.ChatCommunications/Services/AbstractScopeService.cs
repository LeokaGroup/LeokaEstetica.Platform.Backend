using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.Base.Abstractions.Repositories.User;
using LeokaEstetica.Platform.Communications.Abstractions;
using LeokaEstetica.Platform.Database.Abstractions.Communications;
using LeokaEstetica.Platform.Models.Dto.Communications.Output;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.Communications.Services;

/// <summary>
/// Класс реализует методы сервиса абстрактных областей чата.
/// </summary>
internal sealed class AbstractScopeService : IAbstractScopeService
{
    private readonly ILogger<AbstractScopeService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IAbstractScopeRepository _abstractScopeRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="abstractScopeRepository">Репозиторий абстрактных областей.</param>
    public AbstractScopeService(ILogger<AbstractScopeService> logger,
        IUserRepository userRepository,
        IAbstractScopeRepository abstractScopeRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _abstractScopeRepository = abstractScopeRepository;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AbstractScopeOutput>> GetAbstractScopesAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserByEmailAsync(account);

            if (userId == 0)
            {
                throw new InvalidOperationException($"Id пользователя с аккаунтом {account} не найден.");
            }

            var result = await _abstractScopeRepository.GetAbstractScopesAsync(userId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}