namespace LeokaEstetica.Platform.Models.Dto.Output.Document;

/// <summary>
/// Класс выходной модели файлов задачи.
/// </summary>
public class ProjectTaskFileOutput
{
    /// <summary>
    /// Id документа.
    /// </summary>
    public long DocumentId { get; set; }
    
    /// <summary>
    /// Название документа.
    /// </summary>
    public string DocumentName { get; set; }

    /// <summary>
    /// Расширение файла.
    /// </summary>
    public string DocumentExtension { get; set; }
}