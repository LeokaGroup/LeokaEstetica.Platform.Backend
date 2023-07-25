namespace LeokaEstetica.Platform.Models.Dto.Output.Pagination;

/// <summary>
/// TODO: Пока неизвестно, возможно не нужна.
/// Класс с информацией о пагинации.
/// </summary>
public class PaginationInfoOutput
{
    /// <summary>
    /// Номер страницы.
    /// </summary>
    private int PageNumber { get; set; }
    
    /// <summary>
    /// Кол-во всего.
    /// </summary>
    private int Total { get; set; }

    public PaginationInfoOutput(int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        Total = (int)Math.Ceiling(count / (double)pageSize);
    }

    /// <summary>
    /// Признак наличия предыдущей страницы.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Признак наличия следующей страницы.
    /// </summary>
    public bool HasNextPage => PageNumber < Total;
}