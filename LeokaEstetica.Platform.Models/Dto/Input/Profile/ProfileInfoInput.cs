namespace LeokaEstetica.Platform.Models.Dto.Input.Profile;

/// <summary>
/// Класс входной модели информации профиля пользователя для раздела обо мне.
/// </summary>
public class ProfileInfoInput
{
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
}