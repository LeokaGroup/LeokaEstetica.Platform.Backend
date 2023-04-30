namespace LeokaEstetica.Platform.Models.Dto.Base.Moderation.Input;

/// <summary>
/// Базовый класс входной модели замечаний.
/// </summary>
public class BaseRemarkInput
{
    /// <summary>
    /// Название поля.
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// Текст замечания.
    /// </summary>
    public string RemarkText { get; set; }

    /// <summary>
    /// Русское название замечания.
    /// </summary>
    public string RussianName { get; set; }
}