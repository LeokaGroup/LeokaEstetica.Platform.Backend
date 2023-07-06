namespace LeokaEstetica.Platform.Core.Constants;

/// <summary>
/// Класс описывает ключи приложения.
/// </summary>
public static class GlobalConfigKeys
{
    /// <summary>
    /// Класс описывает ключи для Email-писем.
    /// </summary>
    public static class EmailNotifications
    {
        /// <summary>
        /// Ключ для вкл/выкл отправку уведомлений на почту пользователей.
        /// </summary>
        public const string EMAIL_NOTIFICATIONS_DISABLE_MODE_ENABLED = "EmailNotifications.Disable.Mode.Enabled";
    }

    /// <summary>
    /// Класс описывает ключи для чеков.
    /// </summary>
    public static class Receipt
    {
        /// <summary>
        /// Ключ для вкл/откл отправки чеков по возвратам.
        /// </summary>
        public const string SEND_RECEIPT_REFUND_MODE_ENABLED = "SendReceiptRefund.Mode.Enabled";
    }
}