namespace LeokaEstetica.Platform.Models.Dto.Base.ProjectManagement.Paginator;

/// <summary>
/// Класс модели пагинатора задач.
/// </summary>
public class Paginator
{
    public int PageNumber { get; }
    public int TotalPages { get; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="count">Кол-во записей.</param>
    /// <param name="pageNumber">Номер страницы.</param>
    /// <param name="pageSize">Кол-во записей для пагинации.</param>
 
    public Paginator(int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
 
    /// <summary>
    /// Признак наличия предыдущей страницы.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Признак наличия следующей страницы.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}