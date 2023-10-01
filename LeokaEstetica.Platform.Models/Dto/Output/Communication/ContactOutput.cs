namespace LeokaEstetica.Platform.Models.Dto.Output.Communication;

/// <summary>
/// Класс выходной модели контактов.
/// </summary>
public class ContactOutput
{
    /// <summary>
    /// PK.
    /// </summary>
    public short ContactId { get; set; }

    /// <summary>
    /// Название поля контактов.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание поля контактов.
    /// </summary>
    public string Description { get; set; }
}