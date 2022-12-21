namespace LeokaEstetica.Platform.Access.Exceptions;

/// <summary>
/// Исключение возникает, когда нет нужной роли для модерации.
/// </summary>
public class NotAvailableAccessModerationRoleException : UnauthorizedAccessException
{
    public NotAvailableAccessModerationRoleException(string account) : base(
        $"У пользователя {account} нет доступа к модерации. Возможно отсутствует роль либо она отключена.")
    {
    }
}