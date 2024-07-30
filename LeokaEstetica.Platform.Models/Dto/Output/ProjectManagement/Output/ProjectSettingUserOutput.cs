using System.Globalization;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement.Output;

/// <summary>
/// Класс выходной модели пользователя в настройках проекта.
/// </summary>
public class ProjectSettingUserOutput
{
    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

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
    public string? SecondName { get; set; }
    
    /// <summary>
    /// Полное ФИО.
    /// </summary>
    public string FullName => FirstName + " " + LastName + " " + (SecondName ?? string.Empty);

    /// <summary>
    /// Email пользователя.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Дата последней авторизации на платформе.
    /// </summary>
    public DateTime LastAutorization { get; set; }

    /// <summary>
    /// Признак владельца проекта.
    /// </summary>
    public bool IsOwner { get; set; }

    /// <summary>
    /// Роль пользователя.
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Отображаемая дата последней авторизации на платформе.
    /// </summary>
    public string DisplayLastAutorization => LastAutorization.ToString("G", CultureInfo.GetCultureInfo("ru"));
}