using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Moderation.Models.Dto.Input.Messaging.Mail;

/// <summary>
/// TODO: Есть дубль в проекте Messaging, потом отрефачить.
/// TODO: Из-за референсов нельзя переиспользовать, цикличные ссылки.
/// Класс описывает параметры для интеграции сервиса Mailopost для отправки Email-сообщений.
/// </summary>
[Serializable]
public class MailopostInput
{
    /// <summary>
    /// Адрес отправителя.
    /// </summary>
    [JsonProperty("from_email")]
    public string FromEmail { get; set; }

    /// <summary>
    /// Название отправителя.
    /// </summary>
    [JsonProperty("from_name")]
    public string FromName { get; set; }

    /// <summary>
    /// Адрес получателя.
    /// </summary>
    [JsonProperty("to")]
    public string To { get; set; }

    /// <summary>
    /// Заголовок письма.
    /// </summary>
    [JsonProperty("subject")]
    public string Subject { get; set; }

    /// <summary>
    /// Тело письма.
    /// </summary>
    [JsonProperty("text")]
    public string Text { get; set; }

    /// <summary>
    /// Письмо в формате html.
    /// </summary>
    [JsonProperty("html")]
    public string Html { get; set; }

    /// <summary>
    /// Тип тарификации.
    /// </summary>
    [JsonProperty("payment")]
    public string Payment { get; set; }
}