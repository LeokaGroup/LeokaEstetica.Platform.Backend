using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;

/// <summary>
/// Класс элемента дерева Wiki модуля УП.
/// </summary>
public class WikiTreeItem
{
    /// <summary>
    /// Id папки дерева.
    /// </summary>
    public long FolderId { get; set; }
    
    /// <summary>
    /// Название папки/страницы.
    /// </summary>
    [JsonProperty("label")]
    public string? Name { get; set; }

    /// <summary>
    /// Id родителя.
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// Id потомка.
    /// </summary>
    public long? ChildId { get; set; }
    
    /// <summary>
    /// Пользователь, который создал.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Дата создания.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Иконка (папка, страница).
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Id дерева, с которым связана папка.
    /// </summary>
    public long WikiTreeId { get; set; }

    /// <summary>
    /// Id проекта, с которым связано дерево.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Признак наличия дочерних элементов.
    /// </summary>
    public bool HasChildren => Children is not null && Children.Count > 0;

    /// <summary>
    /// Дочерние элементы.
    /// </summary>
    public List<WikiTreeItem>? Children { get; set; }
    
    /// <summary>
    /// Id страницы.
    /// </summary>
    public long PageId { get; set; }

    /// <summary>
    /// Признак страницы.
    /// </summary>
    public bool IsPage => PageId > 0;
    
    /// <summary>
    /// Описание.
    /// </summary>
    public string? PageDescription { get; set; }

    /// <summary>
    /// Признак системной папки (создается не пользователем, а системой).
    /// </summary>
    public bool IsSystem { get; set; }
}