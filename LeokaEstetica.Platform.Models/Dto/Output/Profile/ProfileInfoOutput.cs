using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;
using LeokaEstetica.Platform.Models.Dto.Output.Moderation.Resume;

namespace LeokaEstetica.Platform.Models.Dto.Output.Profile;

/// <summary>
/// Класс выходной модели информации профиля пользователя для раздела обо мне.
/// </summary>
public class ProfileInfoOutput : ResumeRemarkResult, IFrontError
{
    /// <summary>
    /// PK.
    /// </summary>
    public long ProfileInfoId { get; set; }

    /// <summary>
    /// Фамилия.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Имя.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Отчество.
    /// </summary>
    public string Patronymic { get; set; }

    /// <summary>
    /// Отображать ли первую букву фамилии.
    /// </summary>
    public bool IsShortFirstName { get; set; }

    /// <summary>
    /// Ссылка на телегу либо ник.
    /// </summary>
    public string Telegram { get; set; }

    /// <summary>
    /// Ватсап номер телефона.
    /// </summary>
    public string WhatsApp { get; set; }

    /// <summary>
    /// Ссылка на ВК либо ник.
    /// </summary>
    public string Vkontakte { get; set; }

    /// <summary>
    /// Ссылка на другую соц.сеть.
    /// </summary>
    public string OtherLink { get; set; }

    /// <summary>
    /// Обо мне.
    /// </summary>
    public string Aboutme { get; set; }

    /// <summary>
    /// Должность.
    /// </summary>
    public string Job { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }
    
    /// <summary>
    /// Email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Номер телефона пользователя.
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Флаг успешности.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }

    /// <summary>
    /// Опыт работы.
    /// </summary>
    public string WorkExperience { get; set; }

    /// <summary>
    /// Токен пользователя. Если изменился, то будет заполнен.
    /// </summary>
    public string Token { get; set; }
    
    /// <summary>
    /// Признак изменения почты пользователя. Нужно для повторного создания токена и релогина в системе.
    /// </summary>
    public bool IsEmailChanged { get; set; }

    /// <summary>
    /// Признак незаполненной анкеты.
    /// </summary>
    public bool IsEmptyProfile { get; set; }
}