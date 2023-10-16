using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;

namespace LeokaEstetica.Platform.Processing.Strategies.PaymentSystem;

/// <summary>
/// Класс реализует стратегию платежной системы PayMaster.
/// </summary>
internal class PayMasterStrategy : BasePaymentSystemStrategy
{
    private readonly IPayMasterService _payMasterService;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="payMasterService">Сервис платежной системы PayMaster.</param>
    public PayMasterStrategy(IPayMasterService payMasterService)
    {
        _payMasterService = payMasterService;
    }

    /// <summary>
    /// Метод создает заказ.
    /// </summary>
    /// <param name="publicId">Публичный ключ тарифа.</param>
    /// <param name="account">Аккаунт.</param>
    /// <param name="token">Токен пользователя.</param>
    /// <returns>Данные платежа.</returns>
    public override async Task<ICreateOrderOutput> CreateOrderAsync(Guid publicId, string account, string token)
    {
        var result = await _payMasterService.CreateOrderAsync(publicId, account, token);

        return result;
    }
}