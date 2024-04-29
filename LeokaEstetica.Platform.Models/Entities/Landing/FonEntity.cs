namespace LeokaEstetica.Platform.Models.Entities.Landing;

/// <summary>
/// Класс сопоставляется с таблицей dbo.Fon.
/// </summary>
public class FonEntity
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="text">Описание фона.</param>
    public FonEntity(string text)
    {
        Text = text;
    }

    /// <summary>
    /// PK.
    /// </summary>
    public int FonId { get; set; }

    /// <summary>
    /// Заголовок фона.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Описание фона.
    /// </summary>
    public string Text { get; set; }
}