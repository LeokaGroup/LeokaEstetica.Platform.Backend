using System.Text.RegularExpressions;

namespace LeokaEstetica.Platform.Services.Validators;

/// <summary>
/// Класс валидатора параметров пользователя.
/// </summary>
public static class UserValidator
{
    /// <summary>
    /// Метод валидации формата номера телефона.
    /// </summary>
    /// <param name="phoneNumber">Номер телефона.</param>
    /// <returns>Корректен ли номер телефона.</returns>
    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        return Regex.IsMatch(phoneNumber, @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$");
    }

    /// <summary>
    /// Метод валидации почты.
    /// </summary>
    /// <param name="email">Почта.</param>
    /// <returns>Корректная ли почта.</returns>
    public static bool IsValidEmail(string email)
    {
        var isMatch = Regex.Match(email, "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}", RegexOptions.IgnoreCase);
        
        return isMatch.Success;
    }
}