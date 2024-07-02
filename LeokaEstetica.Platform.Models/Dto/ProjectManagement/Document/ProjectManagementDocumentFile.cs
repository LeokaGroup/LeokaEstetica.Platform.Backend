namespace LeokaEstetica.Platform.Models.Dto.ProjectManagement.Document;

/// <summary>
/// Класс модели файла документа в модуле УП.
/// </summary>
public class ProjectManagementDocumentFile
{
    /// <summary>
    /// Название файла.
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Id проекта.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Id задачи (PK).
    /// </summary>
    public long TaskId { get; set; }

    /// <summary>
    /// Признак наличия у задачи файлов.
    /// </summary>
    public bool IsFilesExists => DocumentId > 0;

    /// <summary>
    /// Id документа.
    /// </summary>
    public long? DocumentId { get; set; }
}