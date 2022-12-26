using LeokaEstetica.Platform.Models.Entities.Communication;
using LeokaEstetica.Platform.Models.Entities.Role;

namespace LeokaEstetica.Platform.Models.Entities.User;

/// <summary>
/// Класс сопоставляется с таблицей пользователей dbo.Users.
/// </summary>
public class UserEntity
{
    public UserEntity()
    {
        DialogMessages = new HashSet<DialogMessageEntity>();
        DialogMembers = new HashSet<DialogMemberEntity>();
        ModerationUserRoles = new HashSet<ModerationUserRoleEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long UserId { get; set; }

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
    /// Подтверждена ли почта пользователя.
    /// </summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>
    /// Хэш пароля.
    /// </summary>
    public string PasswordHash { get; set; }

    /// <summary>
    /// Номер телефона.
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Подтвержден ли номер телефона.
    /// </summary>
    public bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// Включена ли двухфакторная аутентификация.
    /// </summary>
    public bool TwoFactorEnabled { get; set; }

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
    public Guid UserCode { get; set; }

    /// <summary>
    /// Guid для подтверждения почты.
    /// </summary>
    public Guid ConfirmEmailCode { get; set; }

    /// <summary>
    /// Дата начала блокировки.
    /// </summary>
    public DateTime? LockoutEnabledDate { get; set; }

    /// <summary>
    /// Дата окончания блокировки.
    /// </summary>
    public DateTime? LockoutEndDate { get; set; }

    /// <summary>
    /// Сообщения диалога.
    /// </summary>
    public ICollection<DialogMessageEntity> DialogMessages { get; set; }

    /// <summary>
    /// Участники диалога.
    /// </summary>
    public ICollection<DialogMemberEntity> DialogMembers { get; set; }

    /// <summary>
    /// Список ролей модерации пользователя.
    /// </summary>
    public ICollection<ModerationUserRoleEntity> ModerationUserRoles { get; set; }

    /// <summary>
    /// Список комментариев к проекту.
    /// </summary>
    public ICollection<ProjectCommentEntity> ProjectComments { get; set; }
}