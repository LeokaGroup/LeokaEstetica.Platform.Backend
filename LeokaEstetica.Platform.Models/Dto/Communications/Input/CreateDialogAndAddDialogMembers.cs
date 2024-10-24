namespace LeokaEstetica.Platform.Models.Dto.Communications.Input;

/// <summary>
/// Класс входной модели создания диалога и добавления участников в диалог.
/// </summary>
public class CreateDialogAndAddDialogMembers
{
    /// <summary>
    /// Список адресов пользователей, которых добавляют в диалог.
    /// </summary>
    public IEnumerable<string>? MemberEmails { get; set; }

    /// <summary>
    /// Название диалога.
    /// </summary>
    public string? DialogName { get; set; }

    /// <summary>
    /// Тип группировки диалогов.
    /// </summary>
    public string? DialogGroupType { get; set; }

    /// <summary>
    /// Id абстрактной записи. Это может быть либо компании либо проекта и тд.
    /// </summary>
    public long? AbstractId { get; set; }
}