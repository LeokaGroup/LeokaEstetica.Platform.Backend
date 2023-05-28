using FluentValidation.Results;
using LeokaEstetica.Platform.Models.Dto.Common;

namespace LeokaEstetica.Platform.Models.Dto.Output.User;

/// <summary>
/// Класс выходной модели пользователя для регистрации.
/// </summary>
public class UserSignUpOutput : IFrontError
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
    public string SecondName { get; set; }

    /// <summary>
    /// Логин.
    /// </summary>
    public string Login { get; set; }

    /// <summary>
    /// Иконка профиля пользователя.
    /// </summary>
    public string UserIcon { get; set; }

    /// <summary>
    /// Дата регистрации.
    /// </summary>
    public DateTime DateRegister { get; set; }

    /// <summary>
    /// Email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Подтвержден ли пароль.
    /// </summary>
    public bool EmailConfirmed { get; set; }
    
    /// <summary>
    /// Номер телефона.
    /// </summary>
    public string PhoneNumber { get; set; }
    
    /// <summary>
    /// Подтвержден ли номер телефона.
    /// </summary>
    public bool PhoneNumberConfirmed { get; set; }
    
    /// <summary>
    /// Отключена ли блокировка пользователя.
    /// </summary>
    public bool LockoutEnd { get; set; }

    /// <summary>
    /// Включена ли блокировка пользователя.
    /// </summary>
    public bool LockoutEnabled { get; set; }

    /// <summary>
    /// PK.
    /// </summary>
    public string UserCode { get; set; }

    /// <summary>
    /// Guid для подтверждения почты.
    /// </summary>
    public string ConfirmEmailCode { get; set; }

    /// <summary>
    /// Дата начала блокировки.
    /// </summary>
    public DateTime? LockoutEnabledDate { get; set; }

    /// <summary>
    /// Дата окончания блокировки.
    /// </summary>
    public DateTime? LockoutEndDate { get; set; }

    /// <summary>
    /// Список ошибок.
    /// </summary>
    public List<ValidationFailure> Errors { get; set; }
}