namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;
using Newtonsoft.Json;

/// <summary>
/// Класс входной модели планирования спринта.
/// </summary>
public class PlaningSprintInput
{
    /// <summary>
    /// Название спринта.
    /// </summary>
    public string SprintName { get; set; }
    
    /// <summary>
    /// Описание спринта.
    /// </summary>
    public string SprintDescription { get; set; }

    /// <summary>
    /// Дата начала спринта.
    // Если не указана, то спринт будет спланирован, но его нельзя будет начать до момента заполнения дат. 
    /// </summary>
    public DateTime? DateStart { get; set; }
    
    /// <summary>
    /// Дата окончания спринта.
    // Если не указана, то спринт будет спланирован, но его нельзя будет начать до момента заполнения дат. 
    /// </summary>
    public DateTime? DateEnd { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Список Id задач в рамках проекта для добавления их в спринт.
    /// Это может быть задача, эпик, история, ошибка.
    /// </summary>
    public IEnumerable<string> ProjectTaskIds { get; set; }

    /// <summary>
    /// Признак автоматического начала спринта после его планирования.
    /// </summary>
    public bool IsAuthStartSprint { get; set; }

    /// <summary>
    /// Id статуса спринта. Заполняется в процессе планирования спринта.
    /// На фронт не показываем это поле.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? SprintStatus { get; set; }
}