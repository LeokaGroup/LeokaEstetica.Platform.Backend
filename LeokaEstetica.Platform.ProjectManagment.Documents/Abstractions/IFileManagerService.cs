using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи.</param>
    /// <returns>Данные файла.</returns>
    Task<FileContentResult> DownloadFileAsync(string fileName, long projectId, long taskId);
    
    /// <summary>
    /// Метод удаляет файл с сервера по SFTP.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи.</param>
    Task RemoveFileAsync(string fileName, long projectId, long taskId);

    /// <summary>
    /// Метод получает изображение аватара пользователя.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id пользователя.</param>
    /// <param name="isNoPhoto">Признак необходимости подгрузить дефолтное изображение..</param>
    /// <returns>Данные файла.</returns>
    Task<FileContentResult> GetUserAvatarFileAsync(string fileName, long projectId, long userId, bool isNoPhoto);
}