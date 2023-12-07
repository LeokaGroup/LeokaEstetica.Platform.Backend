namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс списка вложенных элементов хидера модуля УП (меню создания, т.е. кнопка создать).
/// </summary>
public class ProjectManagmentHeaderCreateItems
{
    /// <summary>
    /// Список элементов.
    /// </summary>
    public List<CreateItems> Items { get; set; }   
}

public class CreateItems
{
    /// <summary>
    /// Название пункта хидера.
    /// </summary>
    public string ItemName { get; set; }
    
    /// <summary>
    /// Порядковый номер элемента меню.
    /// </summary>
    public int Position { get; set; }
}