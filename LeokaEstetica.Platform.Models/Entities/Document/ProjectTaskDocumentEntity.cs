using System.ComponentModel.DataAnnotations.Schema;
using LeokaEstetica.Platform.Models.Enums;
using LeokaEstetica.Platform.Models.Extensions;

namespace LeokaEstetica.Platform.Models.Entities.Document;

/// <summary>
/// Класс сопоставляется с таблицей документов задач проекта.
/// </summary>
public class ProjectTaskDocumentEntity
{
    /// <summary>
    /// PK. Id документа.
    /// </summary>
    public long DocumentId { get; set; }

    /// <summary>
    /// Значение типа документа.
    /// </summary>
    public DocumentTypeEnum DocumentTypeValue { get; set; }
    
    /// <summary>
    /// Тип документа.
    /// </summary>
    [NotMapped]
    public IEnum DocumentType
    {
        get => new Enums.Enum(Enums.Enum.LinkType)
        {
            Value = DocumentTypeValue.ToString().ToSnakeCase()
        };

        set => DocumentTypeValue = System.Enum.Parse<DocumentTypeEnum>(value.Value.ToPascalCase());
    }

    /// <summary>
    /// Название документа.
    /// </summary>
    public string DocumentName { get; set; }

    /// <summary>
    /// Расширение файла.
    /// </summary>
    public string DocumentExtension { get; set; }
    
    /// <summary>
    /// Дата создания документа.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Дата обновления документа.
    /// </summary>
    public DateTime Updated { get; set; }
    
    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }
    
    /// <summary>
    /// Id задачи.
    /// </summary>
    public long TaskId { get; set; }
}