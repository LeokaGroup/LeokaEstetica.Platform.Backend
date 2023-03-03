namespace LeokaEstetica.Platform.Services.Consts;

/// <summary>
/// Класс описывает константы валидации.
/// </summary>
public static class ValidationConsts
{
    /// <summary>
    /// Если пустое имя.
    /// </summary>
    public const string EMPTY_FIRST_NAME_ERROR = "Имя должно быть заполнено.";

    /// <summary>
    /// Если пустая фамилия.
    /// </summary>
    public const string EMPTY_LAST_NAME_ERROR = "Фамилия должна быть заполнена.";

    /// <summary>
    /// Если пустая информация о себе.
    /// </summary>
    public const string EMPTY_ABOUTME_ERROR = "Информация о себе должна быть заполнена.";

    /// <summary>
    /// Если не заполнена почта пользователя.
    /// </summary>
    public const string EMPTY_EMAIL_ERROR = "Email пользователя должен быть заполнен.";

    /// <summary>
    /// Если не заполнен номер телефона пользователя.
    /// </summary>
    public const string EMPTY_PHONE_NUMBER_ERROR = "Номер телефона пользователя должнен быть заполнен.";

    /// <summary>
    /// Если некорректная почта пользователя.
    /// </summary>
    public const string NOT_VALID_EMAIL_ERROR = "Email имеет некорректный формат.";

    /// <summary>
    /// Если некорректный номер телефона пользователя.
    /// </summary>
    public const string NOT_VALID_PHONE_NUMBER_ERROR = "Номер телефона имеет некорректный формат.";

    /// <summary>
    /// Если не заполнен пароль.
    /// </summary>
    public const string EMPTY_PASSWORD_ERROR = "Пароль не может быть пустым.";

    /// <summary>
    /// Если не передали текст для поиска пользователя.
    /// </summary>
    public const string NOT_VALID_INVITE_PROJECT_TEAM_USER =
        "Текст для добавления пользователя в команду проекта не передан.";

    /// <summary>
    /// Если не передали тип приглашения.
    /// </summary>
    public const string NOT_VALID_INVITE_TYPE = "Тип приглашения не передан.";

    /// <summary>
    /// Если не передали ProjectId.
    /// </summary>
    public const string NOT_VALID_INVITE_PROJECT_TEAM_PROJECT_ID =
        "ProjectId для добавления в команду проекта не передан.";

    /// <summary>
    /// Если не передали VacancyId.
    /// </summary>
    public const string NOT_VALID_INVITE_PROJECT_TEAM_VACANCY_ID =
        "VacancyId для добавления в команду проекта не передан.";
        
    /// <summary>
    /// Если Id проекта невалидный.
    /// </summary>
    public const string NOT_VALID_PROJECT_ID = "Невалидный Id проекта. ProjectId был ";
    
    /// <summary>
    /// Если некорректный Id пользователя.
    /// </summary>
    public const string ACCESS_NOT_VALID_USER_ID = "Id пользователя <= 0.";
}