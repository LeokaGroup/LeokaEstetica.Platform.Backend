namespace LeokaEstetica.Platform.Models.Entities.Knowlege;

/// <summary>
/// Класс сопоставляется с таблицей подкатегорий Knowledge.KnowledgeSubCategories
/// </summary>
public class KnowledgeSubCategoryEntity
{
    public KnowledgeSubCategoryEntity()
    {
        KnowledgeSubCategoryThemes = new HashSet<KnowledgeSubCategoryThemeEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long SubCategoryId { get; set; }

    /// <summary>
    /// Id категории.
    /// </summary>
    public long CategoryId { get; set; }

    /// <summary>
    /// FK на категорию.
    /// </summary>
    public KnowledgeCategoryEntity KnowledgeCategory { get; set; }

    /// <summary>
    /// Позиция.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Заголовок типа подкатегории.
    /// </summary>
    public string SubCategoryTypeName { get; set; }

    /// <summary>
    /// Системное название заголовка типа подкатегории.
    /// </summary>
    public string SubCategoryTypeSysName { get; set; }

    /// <summary>
    /// Id темы подкатегории.
    /// </summary>
    public long SubCategoryThemeId { get; set; }

    /// <summary>
    /// FK на тему подкатегории.
    /// </summary>
    public KnowledgeSubCategoryThemeEntity KnowledgeSubCategoryTheme { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ICollection<KnowledgeSubCategoryThemeEntity> KnowledgeSubCategoryThemes { get; set; }
}