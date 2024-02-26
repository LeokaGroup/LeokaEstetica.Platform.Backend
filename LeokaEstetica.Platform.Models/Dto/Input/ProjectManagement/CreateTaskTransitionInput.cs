namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement;

/// <summary>
/// Класс входной модели создания перехода.
/// </summary>
public class CreateTaskTransitionInput
{
    /// <summary>
    /// Название перехода.
    /// </summary>
    public string TransitionName { get; set; }
    
    /// <summary>
    /// Id статуса, из которого переход.
    /// </summary>
    public long FromStatusId { get; set; }

    /// <summary>
    /// Id статуса, в который переход.
    /// </summary>
    public long ToStatusId { get; set; }
}