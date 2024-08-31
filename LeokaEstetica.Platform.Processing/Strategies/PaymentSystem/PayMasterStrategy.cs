// using LeokaEstetica.Platform.Models.Dto.Base.Commerce;
// using LeokaEstetica.Platform.Models.Dto.Output.Commerce.Base.Output;
// using LeokaEstetica.Platform.Processing.Abstractions.PayMaster;
// using LeokaEstetica.Platform.Processing.Enums;
//
// namespace LeokaEstetica.Platform.Processing.Strategies.PaymentSystem;
//
// /// <summary>
// /// Класс реализует стратегию платежной системы PayMaster.
// /// </summary>
// internal class PayMasterStrategy : BasePaymentSystemStrategy
// {
//     private readonly IPayMasterService _payMasterService;
//     
//     /// <summary>
//     /// Конструктор.
//     /// </summary>
//     /// <param name="payMasterService">Сервис платежной системы PayMaster.</param>
//     public PayMasterStrategy(IPayMasterService payMasterService)
//     {
//         _payMasterService = payMasterService;
//     }
//
//     #region Публичные методы.
//
//     /// <summary>
//     /// Метод создает заказ.
//     /// </summary>
//     /// <param name="publicId">Публичный ключ тарифа.</param>
//     /// <param name="account">Аккаунт.</param>
//     /// <returns>Данные платежа.</returns>
//     public override async Task<ICreateOrderOutput> CreateOrderAsync(Guid publicId, string account)
//     {
//         var result = await _payMasterService.CreateOrderAsync(publicId, account);
//
//         return result;
//     }
//
//     /// <summary>
//     /// Метод проверяет статус платежа в ПС.
//     /// </summary>
//     /// <param name="paymentId">Id платежа.</param>
//     /// <returns>Статус платежа.</returns>
//     public override async Task<PaymentStatusEnum> CheckOrderStatusAsync(string paymentId)
//     {
//         var result = await _payMasterService.CheckOrderStatusAsync(paymentId);
//
//         return result;
//     }
//
//     public override Task ConfirmPaymentAsync(string paymentId, Amount amount)
//     {
//         throw new NotImplementedException("Подтверждение платежа не реализовано в ПС PayMaster.");
//     }
//
//     #endregion
//
//     #region Приватные методы.
//
//     
//
//     #endregion
// }