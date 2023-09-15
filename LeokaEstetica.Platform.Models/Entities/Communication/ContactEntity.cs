namespace LeokaEstetica.Platform.Models.Entities.Communication;

/// <summary>
/// Класс сопоставляется с таблицей контактов Communications.Contacts.
/// </summary>
public class ContactEntity
{
    /// <summary>
    /// PK.
    /// </summary>
    public int ContactId { get; set; }

    /// <summary>
    /// Название поля контактов.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание поля контактов.
    /// </summary>
    public string Description { get; set; }
}