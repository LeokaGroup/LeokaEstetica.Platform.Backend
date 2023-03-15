namespace LeokaEstetica.Platform.Models.Entities.Knowlege;

/// <summary>
/// Класс сопоставляется с таблицей категорий БЗ Knowledge.KnowledgeCategories.
/// </summary>
public class KnowledgeCategoryEntity
{
    public KnowledgeCategoryEntity()
    {
        KnowledgeSubCategories = new HashSet<KnowledgeSubCategoryEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long CategoryId { get; set; }

    /// <summary>
    /// Заголовок категории.
    /// </summary>
    public string CategoryTitle { get; set; }

    /// <summary>
    /// FK на таблицу подкатегорий.
    /// </summary>
    public long SubCategoryId { get; set; }

    /// <summary>
    /// Название типа категории.
    /// </summary>
    public string SubCategoryTypeName { get; set; }

    /// <summary>
    /// Системное название типа категории.
    /// </summary>
    public string CategoryTypeSysName { get; set; }

    /// <summary>
    /// Признак нахождения в топе стартовой таблицы.
    /// </summary>
    public bool IsTop { get; set; }
    
    /// <summary>
    /// Позиция.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Id стартовой таблицы БЗ.
    /// </summary>
    public long StartId { get; set; }

    /// <summary>
    /// Стартовая таблица БЗ.
    /// </summary>
    public KnowledgeStartEntity KnowledgeStart { get; set; }

    /// <summary>
    /// Список подкатегорий.
    /// </summary>
    public ICollection<KnowledgeSubCategoryEntity> KnowledgeSubCategories { get; set; }
}