namespace LeokaEstetica.Platform.Processing.Consts;

/// <summary>
/// Класс с api-url платежной системы PayMaster.
/// </summary>
public static class ApiConsts
{
    /// <summary>
    /// Создание платежа в ПС.
    /// </summary>
    public const string CREATE_PAYMENT = "https://paymaster.ru/api/v2/invoices";

    /// <summary>
    /// Проверка статуса платежа.
    /// </summary>
    public const string CHECK_PAYMENT_STATUS = "https://paymaster.ru/api/v2/payments/";

    /// <summary>
    /// Создание возврата в ПС.
    /// </summary>
    public const string CREATE_REFUND = "https://paymaster.ru/api/v2/refunds";
}