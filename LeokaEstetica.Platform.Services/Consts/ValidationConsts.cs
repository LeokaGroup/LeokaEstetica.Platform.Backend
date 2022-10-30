namespace LeokaEstetica.Platform.Services.Consts;

/// <summary>
/// Класс описывает константы валидации.
/// </summary>
public static class ValidationConsts
{
    /// <summary>
    /// Если пустое имя.
    /// </summary>
    public const string EMPTY_FIRST_NAME_ERROR = "Имя должно быть заполнено!";
    
    /// <summary>
    /// Если пустая фамилия.
    /// </summary>
    public const string EMPTY_LAST_NAME_ERROR = "Фамилия должна быть заполнена!";
    
    /// <summary>
    /// Если пустая информация о себе.
    /// </summary>
    public const string EMPTY_ABOUTME_ERROR = "Информация о себе должна быть заполнена!";
    
    /// <summary>
    /// Если не заполнена почта пользователя.
    /// </summary>
    public const string EMPTY_EMAIL_ERROR = "Email пользователя должен быть заполнен!";
    
    /// <summary>
    /// Если не заполнен номер телефона пользователя.
    /// </summary>
    public const string EMPTY_PHONE_NUMBER_ERROR = "Номер телефона пользователя должнен быть заполнен!";
    
    /// <summary>
    /// Если некорректная почта пользователя.
    /// </summary>
    public const string NOT_VALID_EMAIL_ERROR = "Email имеет некорректный формат!";
    
    /// <summary>
    /// Если некорректный номер телефона пользователя.
    /// </summary>
    public const string NOT_VALID_PHONE_NUMBER_ERROR = "Номер телефона имеет некорректный формат!";

    /// <summary>
    /// Если не заполнен пароль.
    /// </summary>
    public const string EMPTY_PASSWORD_ERROR = "Пароль не может быть пустым!";
}