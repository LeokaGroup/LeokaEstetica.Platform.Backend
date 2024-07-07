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
    private const string _projectTaskFiles = "project_task_files_";

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="mongoDb">Инстанс монги.</param>
    public MongoDbRepository(IMongoDatabase mongoDb)
    {
        _mongoDb = mongoDb;
    }

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
        
        // TODO: Не очень удобно так формировать название файлов, но отрефачим, когда разберемся,
        // TODO: как удобнее в монге хранить в коллекции список документов, тогда станет понятнее как лучше называть.
        // Путь к файлам задач проекта.
        var userProjectPathBuilder = new StringBuilder(string.Concat(_projectTaskFiles, projectId));
        userProjectPathBuilder.Append('_');
        userProjectPathBuilder.Append(taskId);
        userProjectPathBuilder.Append('_');

        var gridFS = new GridFSBucket(_mongoDb);

        foreach (var f in files)
        {
            await using var stream = f.OpenReadStream();
            var fileName = string.Concat(userProjectPathBuilder, Path.GetFileName(f.FileName));
            
            await gridFS.UploadFromStreamAsync(fileName, stream);
        }
    }

    /// <inheritdoc />
    public Task<FileContentResult> DownloadFileAsync(string fileName, long projectId, long taskId)
    {
        throw new NotImplementedException();
    }
}