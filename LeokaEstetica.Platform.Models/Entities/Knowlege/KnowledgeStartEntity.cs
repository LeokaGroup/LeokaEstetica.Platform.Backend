namespace LeokaEstetica.Platform.Models.Entities.Knowlege;

/// <summary>
/// Класс сопоставляется с таблицей Knowledge.KnowledgeStart.
/// </summary>
public class KnowledgeStartEntity
{
    public KnowledgeStartEntity()
    {
        KnowledgeCategories = new HashSet<KnowledgeCategoryEntity>();
    }

    /// <summary>
    /// PK.
    /// </summary>
    public long StartId { get; set; }

    /// <summary>
    /// Название категории.
    /// </summary>
    public string CategoryTitle { get; set; }

    /// <summary>
    /// Название категории.
    /// </summary>
    public string CategoryTypeName { get; set; }

    /// <summary>
    /// Системное название категории.
    /// </summary>
    public string CategoryTypeSysName { get; set; }

    /// <summary>
    /// Название подкатегории.
    /// </summary>
    public string SubCategoryTitle { get; set; }

    /// <summary>
    /// Позиция.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Список топовых категорий.
    /// Они отображаются первыми на стартовой странице БЗ.
    /// </summary>
    public string TopCategories { get; set; }

    /// <summary>
    /// Список категорий.
    /// </summary>
    public ICollection<KnowledgeCategoryEntity> KnowledgeCategories { get; set; }
}