using LeokaEstetica.Platform.Models.Enums;

namespace LeokaEstetica.Platform.Core.Exceptions;

/// <summary>
/// Исключение возникает, если не передали тип хидера для получения его списка меню.
/// </summary>
public class EmptyHeaderTypeException : ArgumentNullException
{
    public EmptyHeaderTypeException(HeaderTypeEnum headerType) : base($"Тип хидера {headerType} не существует")
    {
    }
}