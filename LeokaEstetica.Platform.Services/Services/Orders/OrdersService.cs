using LeokaEstetica.Platform.Core.Exceptions;
using LeokaEstetica.Platform.Database.Abstractions.Orders;
using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Models.Entities.Commerce;
using LeokaEstetica.Platform.Services.Abstractions.Orders;
using Microsoft.Extensions.Logging;

namespace LeokaEstetica.Platform.Services.Services.Orders;

/// <summary>
/// Класс реализует методы сервиса заказов.
/// </summary>
public class OrdersService : IOrdersService
{
    private readonly ILogger<OrdersService> _logger;
    private readonly IOrdersRepository _ordersRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логер.</param>
    /// <param name="ordersRepository">Репозиторий заказов пользователя.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    public OrdersService(ILogger<OrdersService> logger, 
        IOrdersRepository ordersRepository, 
        IUserRepository userRepository)
    {
        _logger = logger;
        _ordersRepository = ordersRepository;
        _userRepository = userRepository;
    }

    #region Публичные методы.

    /// <summary>
    /// Метод получает список заказов пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список заказов пользователя.</returns>
    public async Task<IEnumerable<OrderEntity>> GetUserOrdersAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserIdByEmailAsync(account);
            
            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }
            
            var result = await _ordersRepository.GetUserOrdersAsync(userId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает детали заказа по его Id.
    /// </summary>
    /// <param name="orderId">Id заказа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Детали заказа.</returns>
    public async Task<OrderEntity> GetOrderDetailsAsync(long orderId, string account)
    {
        try
        {
            var userId = await _userRepository.GetUserIdByEmailAsync(account);
            
            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var result = await _ordersRepository.GetOrderDetailsAsync(orderId, userId);

            if (result is null)
            {
                var ex = new InvalidOperationException($"Не удалось получить детали заказа. Id заказа: {orderId}");
                throw ex;
            }

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод получает список транзакций по заказам пользователя.
    /// </summary>
    /// <param name="account">Аккаунт.</param>
    /// <returns>Список транзакций.</returns>
    public async Task<IEnumerable<HistoryEntity>> GetHistoryAsync(string account)
    {
        try
        {
            var userId = await _userRepository.GetUserIdByEmailAsync(account);
            
            if (userId <= 0)
            {
                var ex = new NotFoundUserIdByAccountException(account);
                throw ex;
            }

            var result = await _ordersRepository.GetHistoryAsync(userId);

            return result;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    #endregion

    #region Приватные методы.

    

    #endregion
}