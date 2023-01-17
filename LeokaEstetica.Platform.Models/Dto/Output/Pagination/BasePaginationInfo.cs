namespace LeokaEstetica.Platform.Models.Dto.Output.Pagination;

public class BasePaginationInfo
{
    /// <summary>
    /// Информация о пагинации.
    /// </summary>
    public PaginationInfoOutput PaginationInfo { get; set; }

    /// <summary>
    /// Признак отображения пагинации.
    /// </summary>
    public bool IsVisiblePagination { get; set; }
}