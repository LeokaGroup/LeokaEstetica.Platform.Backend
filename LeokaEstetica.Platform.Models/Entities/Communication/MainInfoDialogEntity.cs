namespace LeokaEstetica.Platform.Models.Entities.Communication;

/// <summary>
/// Класс сопоставляется с таблицей диалогов Communications.MainInfoDialogs.
/// </summary>
public class MainInfoDialogEntity
{
    public MainInfoDialogEntity()
    {
        DialogMessages = new HashSet<DialogMessageEntity>();
        DialogMembers = new HashSet<DialogMemberEntity>();
    }

    /// <summary>
    /// Id диалога.
    /// </summary>
    public long DialogId { get; set; }

    /// <summary>
    /// Название диалога.
    /// </summary>
    public string DialogName { get; set; }

    /// <summary>
    /// Дата создания диалога.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Сообщения диалога.
    /// </summary>
    public ICollection<DialogMessageEntity> DialogMessages { get; set; }

    /// <summary>
    /// Участники диалога.
    /// </summary>
    public ICollection<DialogMemberEntity> DialogMembers { get; set; }
}