namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс результата выходной модели хидера модуля УП (управление проектами).
/// </summary>
public class ProjectManagmentHeaderResult
{
    /// <summary>
    /// Название.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Уникальный идентификатор (обычно системное название).
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// Вложенные элементы.
    /// </summary>
    public IEnumerable<ProjectManagmentHeader> Items { get; set; }
}

public class ProjectManagmentHeader
{
    /// <summary>
    /// Название.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Вложенные элементы.
    /// </summary>
    public IEnumerable<ProjectManagmentHeaderItems> Items { get; set; }
    
    /// <summary>
    /// Уникальный идентификатор (обычно системное название).
    /// </summary>
    public string Id { get; set; }
}

public class ProjectManagmentHeaderItems
{
    /// <summary>
    /// Название.
    /// </summary>
    public string Label { get; set; }
    
    /// <summary>
    /// Уникальный идентификатор (обычно системное название).
    /// </summary>
    public string Id { get; set; }
}