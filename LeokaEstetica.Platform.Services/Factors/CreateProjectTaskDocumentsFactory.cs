using LeokaEstetica.Platform.Models.Entities.Document;
using Microsoft.AspNetCore.Http;

namespace LeokaEstetica.Platform.Services.Factors;

/// <summary>
/// Класс факторки создает результаты документов файлов задачи.
/// </summary>
public static class CreateProjectTaskDocumentsFactory
{
    /// <summary>
    /// Метод создает результат для создания документов задачи проекта.
    /// </summary>
    /// <param name="files">Файлы задачи проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи.</param>
    /// <returns>Результат для создания документов задачи проекта.</returns>
    public static IEnumerable<ProjectTaskDocumentEntity> CreateProjectTaskDocuments(IFormFileCollection files,
        long projectId, long taskId)
    {
        var result = new List<ProjectTaskDocumentEntity>();
        
        foreach (var f in files)
        {
            result.Add(new ProjectTaskDocumentEntity
            {
                DocumentName = f.FileName,
                DocumentExtension = Path.GetExtension(f.FileName),
                ProjectId = projectId,
                TaskId = taskId
            });
        }

        return result;
    }
}