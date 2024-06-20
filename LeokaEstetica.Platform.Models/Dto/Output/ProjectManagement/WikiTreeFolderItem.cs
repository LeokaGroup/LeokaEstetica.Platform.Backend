using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;

/// <summary>
/// Класс элемента папки дерева Wiki модуля УП.
/// </summary>
public class WikiTreeFolderItem
{
    /// <summary>
    /// Id папки дерева.
    /// </summary>
    public long FolderId { get; set; }
    
    /// <summary>
    /// Название.
    /// </summary>
    [JsonProperty("label")]
    public string? FolderName { get; set; }

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
    public List<WikiTreePageItem>? Children { get; set; }
}