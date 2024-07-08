namespace LeokaEstetica.Platform.Models.Dto.Output.Resume;

/// <summary>
/// Класс выходной модели пагинации резюме.
/// </summary>
public class PaginationResumeOutput
{
    /// <summary>
    /// Список резюме.
    /// </summary>
    public List<UserInfoOutput>? Resumes { get; set; }

    /// <summary>
    /// Всего анкет в БД. Это кол-во без анкет, что на модерации, отклонены либо не заполнены.
    /// </summary>
    public long? Total { get; set; }
    
    /// <summary>
    /// Признак отображения пагинации.
    /// </summary>
    public bool IsVisiblePagination { get; set; }
}