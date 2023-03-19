namespace LeokaEstetica.Platform.Models.Dto.Output.Knowledge;

/// <summary>
/// Класс выходной модели БЗ для лендоса.
/// </summary>
public class KnowledgeLandingOutput
{
    /// <summary>
    /// Название темы.
    /// </summary>
    public string SubCategoryThemeTitle { get; set; }

    /// <summary>
    /// Описание темы.
    /// </summary>
    public string SubCategoryThemeText { get; set; }

    /// <summary>
    /// Id темы.
    /// </summary>
    public long SubCategoryThemeId { get; set; }
}