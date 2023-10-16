using LeokaEstetica.Platform.Database.Abstractions.Commerce;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Messaging.Abstractions.Mail;
using LeokaEstetica.Platform.Messaging.Abstractions.RabbitMq;
using Microsoft.Extensions.Configuration;

namespace LeokaEstetica.Platform.Processing.Models;

/// <summary>
/// Класс DI-зависимостей заказов.
/// </summary>
public interface IPaymentOrderReference
{
    /// <summary>
    /// Конфигурация приложения.
    /// </summary>
    public IConfiguration Configuration { get; set; }

    /// <summary>
    /// Репозиторий пользователей.
    /// </summary>
    public ICommerceRepository CommerceRepository { get; set; }

    /// <summary>
    /// Сервис кролика.
    /// </summary>
    public IRabbitMqService RabbitMqService { get; set; }

    /// <summary>
    /// Репозиторий глобал конфига.
    /// </summary>
    public IGlobalConfigRepository GlobalConfigRepository { get; set; }

    /// <summary>
    /// Сервис email-писем.
    /// </summary>
    public IMailingsService MailingsService { get; set; }
}