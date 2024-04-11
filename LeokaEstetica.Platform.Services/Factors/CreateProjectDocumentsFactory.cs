using LeokaEstetica.Platform.Models.Entities.Document;
using Microsoft.AspNetCore.Http;

namespace LeokaEstetica.Platform.Services.Factors;

/// <summary>
/// Класс факторки создает результаты документов файлов.
/// </summary>
public static class CreateProjectDocumentsFactory
{
    /// <summary>
    /// Метод создает результат для создания документов проекта.
    /// </summary>
    /// <param name="files">Файлы задачи проекта.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Результат для создания документов проекта.</returns>
    public static IEnumerable<ProjectDocumentEntity> CreateProjectDocuments(IFormFileCollection files,
        long projectId, long? taskId, long? userId)
    {
        var result = new List<ProjectDocumentEntity>();
        
        foreach (var f in files)
        {
            result.Add(new ProjectDocumentEntity
            {
                DocumentName = f.FileName,
                DocumentExtension = Path.GetExtension(f.FileName),
                ProjectId = projectId,
                TaskId = taskId,
                UserId = userId
            });
        }

        return result;
    }
}