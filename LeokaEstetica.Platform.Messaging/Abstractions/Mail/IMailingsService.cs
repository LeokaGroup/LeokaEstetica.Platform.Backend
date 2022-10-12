namespace LeokaEstetica.Platform.Messaging.Abstractions.Mail;

/// <summary>
/// Абстракция сервиса работы с сообщениями.
/// </summary>
public interface IMailingsService
{
    /// <summary>
    /// Метод отправит подтверждение на почту.
    /// </summary>
    /// <param name="mailTo">Email кому отправить.</param>
    /// <param name="confirmEmailCode">Код подтверждения почты.</param>
    Task SendConfirmEmailAsync(string mailTo, Guid confirmEmailCode);
}