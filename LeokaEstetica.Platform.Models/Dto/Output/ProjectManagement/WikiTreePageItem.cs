using Newtonsoft.Json;

namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagement;

/// <summary>
/// Класс элемента страницы Wiki модуля УП.
/// </summary>
public class WikiTreePageItem
{
    /// <summary>
    /// Id страницы.
    /// </summary>
    public long PageId { get; set; }

    /// <summary>
    /// Название.
    /// </summary>
    [JsonProperty("label")]
    public string? PageName { get; set; }

    /// <summary>
    /// Описание.
    /// </summary>
    public string? PageDescription { get; set; }
    
    /// <summary>
    /// Id дерева, с которым связана папка.
    /// </summary>
    public long WikiTreeId { get; set; }
    
    /// <summary>
    /// Пользователь, который создал.
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// Дата создания.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Id папки дерева.
    /// </summary>
    public long FolderId { get; set; }
    
    /// <summary>
    /// Иконка (папка, страница).
    /// </summary>
    public string? Icon { get; set; }
}