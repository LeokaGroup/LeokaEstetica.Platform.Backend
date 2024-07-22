namespace LeokaEstetica.Platform.Models.Dto.Output.Resume;

/// <summary>
/// Класс выходной модели анкет.
/// </summary>
public class UserInfoOutput
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ProfileInfoId { get; set; }

    /// <summary>
    /// Фамилия.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Имя.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Отчество.
    /// </summary>
    public string? Patronymic { get; set; }

    /// <summary>
    /// Отображать ли первую букву фамилии.
    /// </summary>
    public bool IsShortFirstName { get; set; }

    /// <summary>
    /// Ссылка на телегу либо ник.
    /// </summary>
    public string? Telegram { get; set; }

    /// <summary>
    /// Ватсап номер телефона.
    /// </summary>
    public string? WhatsApp { get; set; }

    /// <summary>
    /// Ссылка на ВК либо ник.
    /// </summary>
    public string? Vkontakte { get; set; }

    /// <summary>
    /// Ссылка на другую соц.сеть.
    /// </summary>
    public string? OtherLink { get; set; }

    /// <summary>
    /// Обо мне.
    /// </summary>
    public string? Aboutme { get; set; }

    /// <summary>
    /// Должность.
    /// </summary>
    public string? Job { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }
    
    /// <summary>
    /// Опыт работы.
    /// </summary>
    public string? WorkExperience { get; set; }
    
    /// <summary>
    /// Код пользователя.
    /// </summary>
    public Guid UserCode { get; set; }

    /// <summary>
    /// Признак выделения цветом.
    /// </summary>
    public bool IsSelectedColor { get; set; }
    
    /// <summary>
    /// Цвет тега.
    /// </summary>
    public string? TagColor { get; set; }

    /// <summary>
    /// Значение тега.
    /// </summary>
    public string? TagValue { get; set; }

    /// <summary>
    /// Почта пользователя.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Телефон пользователя.
    /// </summary>
    public string? PhoneNumber { get; set; }
}