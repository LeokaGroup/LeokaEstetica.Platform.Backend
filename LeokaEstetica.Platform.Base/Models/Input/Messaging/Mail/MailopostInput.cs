using System.Text.Json.Serialization;

namespace LeokaEstetica.Platform.Base.Models.Input.Messaging.Mail;

/// <summary>
/// Класс описывает параметры для интеграции сервиса Mailopost для отправки Email-сообщений.
/// </summary>
[Serializable]
public class MailopostInput
{
    /// <summary>
    /// Адрес отправителя.
    /// </summary>
    [JsonPropertyName("from_email")]
    public string FromEmail { get; set; }

    /// <summary>
    /// Название отправителя.
    /// </summary>
    [JsonPropertyName("from_name")]
    public string FromName { get; set; }

    /// <summary>
    /// Адрес получателя.
    /// </summary>
    [JsonPropertyName("to")]
    public string To { get; set; }

    /// <summary>
    /// Заголовок письма.
    /// </summary>
    [JsonPropertyName("subject")]
    public string Subject { get; set; }

    /// <summary>
    /// Тело письма.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }

    /// <summary>
    /// Письмо в формате html.
    /// </summary>
    [JsonPropertyName("html")]
    public string Html { get; set; }

    /// <summary>
    /// Тип тарификации.
    /// </summary>
    [JsonPropertyName("payment")]
    public string Payment { get; set; }
}