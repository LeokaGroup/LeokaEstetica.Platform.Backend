namespace LeokaEstetica.Platform.Models.Dto.Input.User;

/// <summary>
/// Класс входной модели подтверждения аккаунта.
/// </summary>
public class ConfirmAccountInput
{
    /// <summary>
    /// Код подтверждения, который ранее записан при отправке письма подтверждения аккаунта пользователю.
    /// </summary>
    public Guid ConfirmAccountCode { get; set; }
}