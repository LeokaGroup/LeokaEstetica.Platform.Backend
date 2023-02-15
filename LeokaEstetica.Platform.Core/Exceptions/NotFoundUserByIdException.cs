namespace LeokaEstetica.Platform.Core.Exceptions;

/// <summary>
/// Исключение возникает, если не удалось найти пользователя по его Id.
/// </summary>
public class NotFoundUserByIdException : InvalidOperationException
{
    public NotFoundUserByIdException(long userId) : base($"Пользователь с Id: {userId} не найден!")
    {
    }
}