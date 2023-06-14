using System.Text.Json.Serialization;

namespace LeokaEstetica.Platform.CallCenter.Models.Dto.Input.Messaging.Mail;

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
    public string To { get; set; }

    /// <summary>
    /// Заголовок письма.
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Тело письма.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Письмо в формате html.
    /// </summary>
    public string Html { get; set; }

    /// <summary>
    /// Тип тарификации.
    /// </summary>
    public string Payment { get; set; }
}