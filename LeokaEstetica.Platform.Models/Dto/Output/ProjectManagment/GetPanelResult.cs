namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели элементов меню.
/// </summary>
public class GetPanelResult
{
    /// <summary>
    /// Элементы хидера.
    /// </summary>
    public List<PanelResult> HeaderItems { get; set; }
    
    /// <summary>
    /// Элементы левой выдвижной панели.
    /// </summary>
    public List<PanelResult> PanelItems { get; set; }
}