using LeokaEstetica.Platform.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;

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
    /// <param name="userId">Id пользователя.</param>
    /// <param name="isNoPhoto">Признак необходимости подгрузить дефолтное изображение.</param>
    /// <returns>Изображение аватара пользователя.</returns>
    Task<FileContentResult> GetUserAvatarFileAsync(string fileName, long projectId, long? userId, bool isNoPhoto);
    
    /// <summary>
    /// Метод получает изображения аватара пользователей.
    /// </summary>
    /// <param name="documents">Названия документов и Id пользователей.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <returns>Словарь с изображениями аватаров пользователей.</returns>
    Task<IDictionary<long, FileContentResult>> GetUserAvatarFilesAsync(
        IEnumerable<(long? UserId, string DocumentName, DocumentTypeEnum DocumentType)> documents, long projectId);
    
    /// <summary>
    /// Метод загружает файлы по SFTP на сервер.
    /// </summary>
    /// <param name="files">Файлы для отправки.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="userId">Id пользователя.</param>
    Task UploadUserAvatarFileAsync(IFormFileCollection files, long projectId, long userId);

    /// <summary>
    /// TODO: Доделаем в будущем, пока что загоняем обученные модели вручную на сервер.
    /// Метод загружает на сервер модель нейросети.
    /// </summary>
    /// <param name="model">Модель нейросети (хранит опыт всех эпох нейросети).</param>
    /// <param name="version">Версия модели нейросети.</param>
    /// <param name="modelName">Название модели нейросети.</param>
    Task UploadNetworkModelAsync(ITransformer model, string version, string modelName);
}