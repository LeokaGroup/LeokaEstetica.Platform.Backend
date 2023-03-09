using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.Profile;

/// <summary>
/// Класс выходной модели информации профиля пользователя для раздела обо мне.
/// </summary>
public class ProfileInfoOutput : IFrontError
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
    /// Предупреждение.
    /// </summary>
    public string WarningComment { get; set; }

    /// <summary>
    /// Список ошибок FluentValidation.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }
}