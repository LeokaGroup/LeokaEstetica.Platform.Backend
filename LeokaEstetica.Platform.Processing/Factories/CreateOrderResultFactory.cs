using LeokaEstetica.Platform.Models.Dto.Output.Commerce.PayMaster;

namespace LeokaEstetica.Platform.Processing.Factories;

/// <summary>
/// Класс факторки создания результата платежа.
/// </summary>
public static class CreateOrderResultFactory
{
    /// <summary>
    /// Метод создает результат платежа.
    /// </summary>
    /// <param name="paymentId">Id платежа.</param>
    /// <param name="url">Строка оплаты.</param>
    /// <returns>Результирующая модель.</returns>
    public static CreateOrderOutput Create(string paymentId, string url)
    {
        var result = new CreateOrderOutput
        {
            PaymentId = paymentId,
            Url = url
        };

        return result;
    }
}