using System.Text;
using LeokaEstetica.Platform.Database.MongoDb.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace LeokaEstetica.Platform.Database.MongoDb.Repositories;

/// <summary>
/// Класс реализует методы репозитория работы с MongoDB.
/// </summary>
internal sealed class MongoDbRepository : IMongoDbRepository
{
    private readonly IMongoDatabase _mongoDb;
    
    /// <summary>
    /// Коллекция документов аватара пользователей.
    /// </summary>
    private const string _projectUserAvatar = "project_user_avatar";
    
    /// <summary>
    /// Коллекция документов проекта пользователя.
    /// </summary>
    private const string _projectFiles = "project_files";

    /// <summary>
    /// Билдер для дерева документов проекта.
    /// </summary>
    private readonly StringBuilder _bucketNameBuilder = new();

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="mongoDb">Инстанс монги.</param>
    public MongoDbRepository(IMongoDatabase mongoDb)
    {
        _mongoDb = mongoDb;
    }

    #region Публичные методы.

    /// <inheritdoc />
    public async Task CreateDefaultProjectManagementFilesAsync()
    {
        var projectUserAvatar = _mongoDb.GetCollection<BsonDocument>(_projectUserAvatar);

        // TODO: Подумать, где хранить аватар до появления его в коллекции монги.
        // Если коллекции нет, то заведем ее.
        if (await projectUserAvatar.CountDocumentsAsync("{}") == 0)
        {
        }
    }

    /// <inheritdoc />
    public async Task UploadFilesAsync(IFormFileCollection files, long projectId, long taskId)
    {
        if (!files.Any())
        {
            return;
        }

        // Создаем коллекцию для документов проектов. Если она уже есть, то просто работаем с ней.
        var gridFS = new GridFSBucket(_mongoDb, new GridFSBucketOptions
        {
            BucketName = _projectFiles
        });

        // Заполняем коллекцию документов файлами задачи.
        foreach (var f in files)
        {
            await using var stream = f.OpenReadStream();

            await BuildBucketNameBuilderAsync(projectId, taskId);

            _bucketNameBuilder.Append('_');
            _bucketNameBuilder.Append(Path.GetFileName(f.FileName));

            await gridFS.UploadFromStreamAsync(_bucketNameBuilder.ToString(), stream);
        }
    }

    /// <inheritdoc />
    public async Task<FileContentResult> DownloadFileAsync(string fileName, long projectId, long taskId)
    {
        await BuildBucketNameBuilderAsync(projectId, taskId);

        _bucketNameBuilder.Append('_');
        _bucketNameBuilder.Append(Path.GetFileName(fileName));
        
        var gridFS = new GridFSBucket(_mongoDb, new GridFSBucketOptions
        {
            BucketName = _projectFiles
        });

        var stream = await gridFS.OpenDownloadStreamByNameAsync(_bucketNameBuilder.ToString());

        var byteData = await GetByteArrayAsync(stream);
        var result = new FileContentResult(byteData, "application/octet-stream");

        return result;
    }

    #endregion

    #region Приватные методы.

    /// <summary>
    /// Метод чистит, чтобы перестроить билдер для итераций документов.
    /// </summary>
    /// <param name="projectId">Id проекта.</param>
    /// <param name="taskId">Id задачи.</param>
    private async Task BuildBucketNameBuilderAsync(long projectId, long taskId)
    {
        _bucketNameBuilder.Clear();
        _bucketNameBuilder.Append(_projectFiles);
        _bucketNameBuilder.Append('_');
        _bucketNameBuilder.Append(projectId);
        _bucketNameBuilder.Append('_');
        _bucketNameBuilder.Append(taskId);

        await Task.CompletedTask;
    }
    
    /// <summary>
    /// Метод получит массив байт из потока.
    /// </summary>
    /// <param name="input">Поток.</param>
    /// <returns>Масив байт.</returns>
    private async Task<byte[]> GetByteArrayAsync(Stream input)
    {
        var buffer = new byte[16*1024];
        await using var ms = new MemoryStream();
        var read = 0;

        while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            ms.Write(buffer, 0, read);
        }
                
        return ms.ToArray();
    }

    #endregion
}