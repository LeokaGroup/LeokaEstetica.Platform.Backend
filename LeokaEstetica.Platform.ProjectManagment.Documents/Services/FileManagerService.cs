using System.Net.Sockets;
using LeokaEstetica.Platform.Base.Factors;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.ProjectManagment.Documents.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace LeokaEstetica.Platform.ProjectManagment.Documents.Services;

/// <summary>
/// Класс реализует методы сервиса менеджера работы с файлами.
/// </summary>
internal sealed class FileManagerService : IFileManagerService
{
    private readonly ILogger<FileManagerService> _logger;
    private readonly Lazy<ITransactionScopeFactory> _transactionScopeFactory;
    private readonly Lazy<IGlobalConfigRepository> _globalConfigRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="transactionScopeFactory">Факторка для создания транзакций.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    public FileManagerService(ILogger<FileManagerService> logger,
        Lazy<ITransactionScopeFactory> transactionScopeFactory,
        Lazy<IGlobalConfigRepository> globalConfigRepository)
    {
        _logger = logger;
        _transactionScopeFactory = transactionScopeFactory;
        _globalConfigRepository = globalConfigRepository;
    }

    /// <inheritdoc />
    public async Task UploadFilesAsync(IFormFileCollection files, long projectId, long taskId)
    {
        if (!files.Any())
        {
            return;
        }

        var settings = await _globalConfigRepository.Value.GetFileManagerSettingsAsync();
        
        using var sftpClient = new SftpClient(settings.Host, settings.Port, settings.Login, settings.Password);

        try
        {
            sftpClient.Connect();

            if (!sftpClient.IsConnected)
            {
                throw new InvalidOperationException("Sftp клиент не подключен. Невозможно загрузить файл.");
            }

            var sftpTaskPath = settings.SftpTaskPath;

            // Путь к файлам задач проекта.
            var userProjectPath = string.Concat(sftpTaskPath, projectId);

            // Если папка проекта не существует, то создаем ее.
            if (!sftpClient.Exists(userProjectPath))
            {
                sftpClient.CreateDirectory(userProjectPath);
            }

            var userProjectTaskPath = userProjectPath + "/" + taskId;
            
            // Если папка задачи проекта не существует, то создаем ее.
            if (!sftpClient.Exists(userProjectTaskPath))
            {
                sftpClient.CreateDirectory(userProjectTaskPath);
            }

            using var scope = _transactionScopeFactory.Value.CreateTransactionScope();

            foreach (var f in files)
            {
                var stream = f.OpenReadStream();
                var fileName = f.FileName;
                var fileStreamLength = stream.Length;
                
                _logger.LogInformation($"Загружается файл {0} ({1:N0} байт)", fileName, fileStreamLength);
                
                sftpClient.UploadFile(stream, string.Concat(sftpTaskPath, Path.GetFileName(fileName)));
                
                _logger.LogInformation($"Файл {0} ({1:N0} байт) успешно загружен.", fileName, fileStreamLength);
            }
            
            scope.Complete();
            sftpClient.Disconnect();
        }

        catch (Exception ex) when (ex is SshConnectionException or SocketException or ProxyException)
        {
            _logger.LogError(ex, "Ошибка подключения к серверу по Sftp.");
            sftpClient.Disconnect();
            throw;
        }

        catch (SshAuthenticationException ex)
        {
            _logger.LogError(ex, "Ошибка аутентификации к серверу по Sftp.");
            sftpClient.Disconnect();
            throw;
        }

        catch (SftpPermissionDeniedException ex)
        {
            _logger.LogError(ex, "Ошибка доступа к серверу по Sftp.");
            sftpClient.Disconnect();
            throw;
        }

        catch (SshException ex)
        {
            _logger.LogError(ex, "Ошибка Sftp.");
            sftpClient.Disconnect();
            throw;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке файла на сервер.");
            sftpClient.Disconnect();
            throw;
        }
    }

    /// <inheritdoc />
    public Task DownloadFileAsync(string fileName)
    {
        throw new NotImplementedException();
    }
}