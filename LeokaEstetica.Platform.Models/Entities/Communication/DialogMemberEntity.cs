using LeokaEstetica.Platform.Models.Entities.User;

namespace LeokaEstetica.Platform.Models.Entities.Communication;

/// <summary>
/// Класс сопоставляется с таблицей участников диалога Communications.DialogMembers.
/// </summary>
public class DialogMemberEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public long MemberId { get; set; }

    /// <summary>
    /// Id пользователя.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// FK на пользователей.
    /// </summary>
    public UserEntity User { get; set; }

    /// <summary>
    /// Дата присоединения к диалогу.
    /// </summary>
    public DateTime Joined { get; set; }

    /// <summary>
    /// Id диалога.
    /// </summary>
    public long DialogId { get; set; }

    /// <summary>
    /// FK на диалоги.
    /// </summary>
    public MainInfoDialogEntity Dialog { get; set; }
}