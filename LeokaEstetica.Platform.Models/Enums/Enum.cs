using LeokaEstetica.Platform.Models.Extensions;

namespace LeokaEstetica.Platform.Models.Enums;

/// <summary>
/// Класс для преставления типа SQL Enums в Postgres.
/// </summary>
public class Enum : IEnum
{
    /// <summary>
    /// Тип Enum для типов тегов.
    /// </summary>
    public const string ObjectTagType = "object_tag_type_enum";
    
    /// <summary>
    /// Тип Enum для типов связей задач.
    /// </summary>
    public const string LinkType = "link_type_enum";

    /// <summary>
    /// Тип Enum для типа документа.
    /// </summary>
    public const string DocumentType = "document_type_enum";

    /// <summary>
    /// Тип Enum для типа перехода.
    /// </summary>
    public const string TransitionType = "transition_type_enum";
    
    /// <summary>
    /// Тип Enum для типа заадчи.
    /// </summary>
    public const string TaskType = "task_type_enum";

    /// <summary>
    /// Конструктор по дефолту.
    /// </summary>
    /// <param name="type">Тип Enum в базе данных.</param>
    public Enum(string type = null)
    {
        Type = type;
    }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа тега.</param>
    public Enum(ObjectTagTypeEnum value)
    {
        Type = ObjectTagType;
        Value = value.ToString().ToSnakeCase();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа связи задачи.</param>
    public Enum(LinkTypeEnum value)
    {
        Type = LinkType;
        Value = value.ToString().ToSnakeCase();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа документа.</param>
    public Enum(DocumentTypeEnum value)
    {
        Type = DocumentType;
        Value = value.ToString().ToSnakeCase();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа перехода.</param>
    public Enum(TransitionTypeEnum value)
    {
        Type = TransitionType;
        Value = value.ToString().ToSnakeCase();
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="value">Значение типа задачи.</param>
    public Enum(SearchAgileObjectTypeEnum value)
    {
        Type = TaskType;
        Value = value.ToString().ToSnakeCase();
    }

    /// <inheritdoc/>
    public string Value { get; set; }

    /// <inheritdoc/>
    public string Type { get; }
}