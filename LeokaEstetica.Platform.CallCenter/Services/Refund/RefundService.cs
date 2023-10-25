using System.Runtime.CompilerServices;
using LeokaEstetica.Platform.CallCenter.Abstractions.Refund;
using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Models.Entities.Commerce;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("LeokaEstetica.Platform.Tests")]

namespace LeokaEstetica.Platform.CallCenter.Services.Refund;

/// <summary>
/// Класс реализует методы сервиса возвратов в КЦ.
/// </summary>
internal sealed class RefundService : IRefundService
{
    private readonly ILogger<RefundService> _logger;
    private readonly ICommerceRepository _commerceRepository;

    /// <summary>
    /// Конструктор.
    /// <param name="logger">Логгер.</param>
    /// <param name="commerceRepository">Репозиторий коммерции.</param>
    /// </summary>
    public RefundService(ILogger<RefundService> logger,
        ICommerceRepository commerceRepository)
    {
        _logger = logger;
        _commerceRepository = commerceRepository;
    }

    /// <summary>
    /// Метод получает список возвратов для КЦ, которые не обработаны.
    /// </summary>
    /// <returns>Список необработанных возвратов.</returns>
    public async Task<IEnumerable<RefundEntity>> GetUnprocessedRefundsAsync()
    {
        try
        {
            var result = await _commerceRepository.GetUnprocessedRefundsAsync();

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}