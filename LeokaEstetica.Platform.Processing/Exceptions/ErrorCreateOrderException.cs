namespace LeokaEstetica.Platform.Processing.Exceptions;

/// <summary>
/// Исключения ошибки создания заказа.
/// </summary>
public class ErrorCreateOrderException : Exception
{
    public ErrorCreateOrderException(string orderData) : base($"Ошибка при создании заказа. Данные заказа: {orderData}")
    {
    }
}