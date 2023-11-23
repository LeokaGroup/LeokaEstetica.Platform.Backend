namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс результата выходной модели хидера модуля УП (управление проектами).
/// </summary>
public class ProjectManagmentHeaderResult
{
    /// <summary>
    /// Список без наполненных доп.списков.
    /// </summary>
    public IEnumerable<ProjectManagmentHeaderOutput> ProjectManagmentHeaderItems { get; set; }
    
    /// <summary>
    /// Список вложенных элементов для стратегий представления. 
    /// </summary>

    public IOrderedEnumerable<StrategyItems> StrategyItems { get; set; }

    /// <summary>
    /// Список вложенных элементов для меню создания, т.е. кнопка создать. 
    /// </summary>
    public IOrderedEnumerable<CreateItems> CreateItems { get; set; }

    /// <summary>
    /// Список вложенных элементов для фильтров. 
    /// </summary>
    public IOrderedEnumerable<Filters> Filters { get; set; }
}