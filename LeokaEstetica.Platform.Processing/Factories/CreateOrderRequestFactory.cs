using LeokaEstetica.Platform.Models.Dto.Base.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;
using LeokaEstetica.Platform.Models.Entities.FareRule;
using LeokaEstetica.Platform.Processing.Enums;
using Microsoft.Extensions.Configuration;

namespace LeokaEstetica.Platform.Processing.Factories;

/// <summary>
/// Класс факторки создания запроса создания заказа.
/// </summary>
public static class CreateOrderRequestFactory
{
    /// <summary>
    /// Метод создает модель запроса в ПС.
    /// </summary>
    /// <param name="createOrderInput">Входная модель.</param>
    /// <param name="configuration"></param>
    /// <param name="fareRule">Данные тарифа.</param>
    public static void Create(CreateOrderInput createOrderInput, IConfiguration configuration,
        FareRuleEntity fareRule)
    {
        // Задаем Id мерчанта (магазина).
        createOrderInput.MerchantId = new Guid(configuration["Commerce:PayMaster:MerchantId"]);
        createOrderInput.TestMode = true; // TODO: Добавить управляющий ключ в таблицу конфигов.
        createOrderInput.Invoice = new Invoice
        {
            Description = "Оплата тарифа: " + fareRule.Name
        };
        createOrderInput.Amount = new Amount
        {
            Value = fareRule.Price,
            Currency = PaymentCurrencyEnum.RUB.ToString()
        };
        createOrderInput.PaymentMethod = "BankCard";
    }
}