namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели создания статуса.
/// </summary>
public class CreateTaskStatusInput
{
    /// <summary>
    /// Системное названия статуса, с которым ассоциируется новый статус.
    /// </summary>
    public string AssociationStatusSysName { get; set; }
    
    /// <summary>
    /// Название статуса.
    /// </summary>
    public string StatusName { get; set; }

    /// <summary>
    /// Описание статуса. 
    /// </summary>
    public string StatusDescription { get; set; }
    
    /// <summary>
    /// Id проекта.
    /// </summary>
    public int ProjectId { get; set; }
}