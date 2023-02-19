namespace LeokaEstetica.Platform.Core.Exceptions;

/// <summary>
/// Исключение возникает, если не нашли пользователя по его Id.
/// </summary>
public class NotFoundUserIdByAccountException : Exception
{
    public NotFoundUserIdByAccountException(string account) : base(
        $"Не удалось найти Id пользователя с аккаунтом {account}")
    {
    }
}