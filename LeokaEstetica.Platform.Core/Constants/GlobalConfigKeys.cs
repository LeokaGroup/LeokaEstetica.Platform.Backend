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
        
        /// <summary>
        /// Ключ API для уведомлений на почту.
        /// </summary>
        public const string API_MAIL_URL = "Api.Mail.Url";
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

    /// <summary>
    /// Класс ключей интеграций.
    /// </summary>
    public static class Integrations
    {
        /// <summary>
        /// Класс ключей телеграма.
        /// </summary>
        public static class Telegram
        {
            /// <summary>
            /// Ключ для создания ссылки инвайта в канал уведомлений.
            /// </summary>
            public const string NOTIFICATIONS_BOT_INVITE = "Notifications.Bot.Invite";
            
            /// <summary>
            /// Ключ для ссылки на объект (проект, вакансию).
            /// </summary>
            public const string NOTIFICATIONS_BOT_CREATED_OBJECT_LINK = "Notifications.Bot.CreatedObjectLink";
        }

        /// <summary>
        /// Класс ключей платежных систем.
        /// </summary>
        public static class PaymentSystem
        {
            /// <summary>
            /// Ключ переключения платежной системы.
            /// </summary>
            public const string COMMERCE_PAYMENT_SYSTEM_TYPE_MODE = "Commerce.Payment.System.Type.Mode";

            /// <summary>
            /// Ключ вкл/откл тестового режима в ПС.
            /// </summary>
            public const string COMMERCE_TEST_MODE_ENABLED = "Commerce.Test.Mode.Enabled";

            /// <summary>
            /// Ключ тестовой цены для тестирования оплаты в ПС на реальной цене заказа.
            /// </summary>
            public const string COMMEFCE_TEST_PRICE_MODE_ENABLED_VALUE = "Commerce.Test.Price.Enabled.Value";
            
            /// <summary>
            /// Ключ вкл/откл режим тестовой цены для тестирования оплаты в ПС на реальной цене заказа.
            /// </summary>
            public const string COMMEFCE_TEST_PRICE_MODE_ENABLED = "Commerce.Test.Price.Mode.Enabled";
        }
    }

    /// <summary>
    /// Класс ключей провайдеров аутентификации.
    /// </summary>
    public static class AuthProviderReference
    {
        /// <summary>
        /// Ключ провайдера аутентификации через провайдера ВК.
        /// </summary>
        public const string AUTH_PROVIDER_REFERENCE_VK = "Auth.Provider.Reference.Vk";

        /// <summary>
        /// Ключ редиректа после успешной аутентификации через провайдера ВК.
        /// </summary>
        public const string AUTH_PROVIDER_REDIRECT_REFERENCE_VK = "Auth.Provider.Redirect.Reference.Vk";
        
        /// <summary>
        /// Ключ провайдера аутентификации через провайдера Google.
        /// </summary>
        public const string AUTH_PROVIDER_REFERENCE_GOOGLE = "Auth.Provider.Reference.Google";

        /// <summary>
        /// Ключ редиректа после успешной аутентификации через провайдера Google.
        /// </summary>
        public const string AUTH_PROVIDER_REDIRECT_REFERENCE_GOOGLE = "Auth.Provider.Redirect.Reference.Google";
    }

    /// <summary>
    /// Класс ключей режимов работы фоновых джоб.
    /// </summary>
    public static class JobsMode
    {
        /// <summary>
        /// Ключ вкл/откл режим работы джобы заказов.
        /// </summary>
        public const string ORDERS_JOB_MODE_ENABLED = "Orders.Job.Mode.Enabled";
        
        /// <summary>
        /// Ключ вкл/откл режим работы джобы возвратов.
        /// </summary>
        public const string REFUNDS_JOB_MODE_ENABLED = "Refunds.Job.Mode.Enabled";
    }

    /// <summary>
    /// Класс ключей модуля УП.
    /// </summary>
    public static class ProjectManagment
    {
        /// <summary>
        /// Ключ вкл/откл модуль управления проектами.
        /// </summary>
        public const string PROJECT_MANAGMENT_MODE_ENABLED = "ProjectManagment.Mode.Enabled";
    }

    /// <summary>
    /// Класс ключей настроек проектов.
    /// </summary>
    public static class ConfigSpaceSetting
    {
        /// <summary>
        /// Ключ стратегии представления.
        /// </summary>
        public const string PROJECT_MANAGEMENT_STRATEGY = "ProjectManagement.Strategy";

        /// <summary>
        /// Ключ шаблона.
        /// </summary>
        public const string PROJECT_MANAGMENT_TEMPLATE_ID = "ProjectManagment.TemplateId";
    }
}