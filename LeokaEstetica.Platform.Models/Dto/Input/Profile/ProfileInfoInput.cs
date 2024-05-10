using LeokaEstetica.Platform.Models.Dto.Output.Profile;

namespace LeokaEstetica.Platform.Models.Dto.Input.Profile;

/// <summary>
/// Класс входной модели информации профиля пользователя для раздела обо мне.
/// </summary>
public class ProfileInfoInput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="lastName">Фамилия.</param>
    /// <param name="firstName">Имя.</param>
    /// <param name="aboutme">Обо мне.</param>
    /// <param name="job">Должность.</param>
    /// <param name="email">Почта пользователя.</param>
    /// <param name="phoneNumber">Номер телефона.</param>
    /// <param name="userSkills">Список навыков пользователя, которые он выбрал.</param>
    /// <param name="userIntents">Список целей пользователя, которые он выбрал.</param>
    /// <param name="workExperience">Опыт работы.</param>
    public ProfileInfoInput(string lastName, string firstName, string aboutme, string job, string email,
        string phoneNumber, IEnumerable<SkillInput> userSkills, IEnumerable<IntentOutput> userIntents,
        string workExperience)
    {
        LastName = lastName;
        FirstName = firstName;
        Aboutme = aboutme;
        Job = job;
        Email = email;
        PhoneNumber = phoneNumber;
        UserSkills = userSkills;
        UserIntents = userIntents;
        WorkExperience = workExperience;
    }

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
    public string Aboutme { get; set; }

    /// <summary>
    /// Должность.
    /// </summary>
    public string Job { get; set; }

    /// <summary>
    /// Почта пользователя.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Номер телефона.
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Список навыков пользователя, которые он выбрал.
    /// </summary>
    public IEnumerable<SkillInput> UserSkills { get; set; }

    /// <summary>
    /// Список целей пользователя, которые он выбрал.
    /// </summary>
    public IEnumerable<IntentOutput> UserIntents { get; set; }

    /// <summary>
    /// Опыт работы.
    /// </summary>
    public string WorkExperience { get; set; }
}