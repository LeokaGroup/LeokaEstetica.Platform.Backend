namespace LeokaEstetica.Platform.Processing.Consts;

/// <summary>
/// Класс с api-url платежных систем.
/// </summary>
public static class ApiConsts
{
    /// <summary>
    /// Класс ендпроинтов платежной системы PayMaster.
    /// </summary>
    public static class PayMaster
    {
        /// <summary>
        /// Создание платежа в ПС.
        /// </summary>
        public const string CREATE_PAYMENT = "https://paymaster.ru/api/v2/invoices";

        /// <summary>
        /// Проверка статуса платежа в ПС.
        /// </summary>
        public const string CHECK_PAYMENT_STATUS = "https://paymaster.ru/api/v2/payments/";

        /// <summary>
        /// Создание возврата в ПС.
        /// </summary>
        public const string CREATE_REFUND = "https://paymaster.ru/api/v2/refunds";
    
        /// <summary>
        /// Проверка статуса возврата в ПС.
        /// </summary>
        public const string CHECK_REFUND_STATUS = "https://paymaster.ru/api/v2/refunds/";

        /// <summary>
        /// Создание чека в ПС.
        /// </summary>
        public const string CREATE_RECEIPT = "https://paymaster.ru/api/v2/receipts";
    }

    /// <summary>
    /// Класс ендпроинтов платежной системы ЮKassa.
    /// </summary>
    public static class YandexKassa
    {
        /// <summary>
        /// Создание платежа в ПС.
        /// </summary>
        public const string CREATE_PAYMENT = "https://api.yookassa.ru/v3/payments";
        
        /// <summary>
        /// Проверка статуса платежа в ПС.
        /// </summary>
        public const string CHECK_PAYMENT_STATUS = "https://api.yookassa.ru/v3/payments/";

        /// <summary>
        /// Подтверждение платежа в ПС.
        /// </summary>
        public const string CONFIRM_PAYMENT = "https://api.yookassa.ru/v3/payments/";
    }
}