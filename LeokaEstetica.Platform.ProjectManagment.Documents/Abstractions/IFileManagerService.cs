using Microsoft.AspNetCore.Http;

namespace LeokaEstetica.Platform.ProjectManagment.Documents.Abstractions;

/// <summary>
/// Абстракция сервиса менеджера работы с файлами.
/// </summary>
public interface IFileManagerService
{
    /// <summary>
    /// Метод загружает файлы по SFTP на сервер.
    /// </summary>
    /// <param name="files">Файлы для отправки.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи.</param>
    Task UploadFilesAsync(IFormFileCollection files, long projectId, long taskId);

    /// <summary>
    /// Метод скачивает файл с сервера по SFTP.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    /// <returns>Файл для скачивания фронтом.</returns>
    Task DownloadFileAsync(string fileName);
}