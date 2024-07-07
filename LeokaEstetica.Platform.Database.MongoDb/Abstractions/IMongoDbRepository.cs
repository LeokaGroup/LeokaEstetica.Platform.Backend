using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeokaEstetica.Platform.Database.MongoDb.Abstractions;

/// <summary>
/// Абстракция репозитория работы с MongoDB.
/// </summary>
public interface IMongoDbRepository
{
    /// <summary>
    /// Метод заводит дефолтные коллекции для документов модуля УП.
    /// Дефолтные коллекции - это коллекции со списком аватаров пользователей по дефолту и тд.
    /// </summary>
    Task CreateDefaultProjectManagementFilesAsync();
    
    /// <summary>
    /// Метод загружает файлы В БД MongoDB.
    /// </summary>
    /// <param name="files">Файлы для отправки.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи.</param>
    Task UploadFilesAsync(IFormFileCollection files, long projectId, long taskId);

    /// <summary>
    /// Метод скачивает файл из БД MongoDB.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи.</param>
    /// <returns>Данные файла.</returns>
    Task<FileContentResult> DownloadFileAsync(string fileName, long projectId, long taskId);
    
    /// <summary>
    /// Метод загружает файлы в БД MongoDB.
    /// </summary>
    /// <param name="files">Файлы для отправки.</param>
    Task UploadUserAvatarFileAsync(IFormFileCollection files);
}