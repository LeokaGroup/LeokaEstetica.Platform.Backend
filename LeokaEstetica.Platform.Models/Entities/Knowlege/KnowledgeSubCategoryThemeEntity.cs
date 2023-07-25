namespace LeokaEstetica.Platform.Models.Entities.Knowlege;

/// <summary>
/// Класс сопоставляется с таблицей тем подкатегорий Knowledge.KnowledgeSubCategoriesThemes.
/// </summary>
public class KnowledgeSubCategoryThemeEntity
{
    public KnowledgeSubCategoryThemeEntity()
    {
        KnowledgeSubCategories = new HashSet<KnowledgeSubCategoryEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long SubCategoryThemeId { get; set; }

    /// <summary>
    /// Заголовок темы подкатегории.
    /// </summary>
    public string SubCategoryThemeTitle { get; set; }

    /// <summary>
    /// Подзаголовок темы подкатегории.
    /// </summary>
    public string SubCategoryThemeSubTitle { get; set; }

    /// <summary>
    /// Описание темы подкатегории.
    /// </summary>
    public string SubCategoryThemeText { get; set; }

    /// <summary>
    /// Изображение.
    /// </summary>
    public string SubCategoryThemeImg { get; set; }

    /// <summary>
    /// Позиция.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Id подкатегории.
    /// </summary>
    public long SubCategoryId { get; set; }

    /// <summary>
    /// Список подкатегорий.
    /// </summary>
    public ICollection<KnowledgeSubCategoryEntity> KnowledgeSubCategories { get; set; }

    /// <summary>
    /// FK на подкатегорию.
    /// </summary>
    /// <returns></returns>
    public KnowledgeSubCategoryEntity KnowledgeSubCategory { get; set; }
}