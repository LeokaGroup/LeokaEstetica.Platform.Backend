using System.Net.Sockets;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.ProjectManagment.Documents.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    private readonly Lazy<IGlobalConfigRepository> _globalConfigRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="globalConfigRepository">Репозиторий глобал конфига.</param>
    public FileManagerService(ILogger<FileManagerService> logger,
        Lazy<IGlobalConfigRepository> globalConfigRepository)
    {
        _logger = logger;
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

            var userProjectTaskPath = userProjectPath + "/" + taskId + "/";
            
            // Если папка задачи проекта не существует, то создаем ее.
            if (!sftpClient.Exists(userProjectTaskPath))
            {
                sftpClient.CreateDirectory(userProjectTaskPath);
            }
            
            // Повторно проверяем создалась ли папка проекта.
            if (!sftpClient.Exists(userProjectPath))
            {
                throw new InvalidOperationException("Папка проекта не существует.");
            }
            
            // Повторно проверяем создалась ли папка задачи проекта.
            if (!sftpClient.Exists(userProjectTaskPath))
            {
                throw new InvalidOperationException("Папка задачи проекта не существует.");
            }

            foreach (var f in files)
            {
                var stream = f.OpenReadStream();
                var fileName = f.FileName;
                var fileStreamLength = stream.Length;
                
                _logger.LogInformation($"Загружается файл {0} ({1:N0} байт)", fileName, fileStreamLength);
                
                sftpClient.BufferSize = 4 * 1024;
                sftpClient.UploadFile(stream, string.Concat(userProjectTaskPath, Path.GetFileName(fileName)));
                
                _logger.LogInformation($"Файл {0} ({1:N0} байт) успешно загружен.", fileName, fileStreamLength);
            }
        }

        catch (Exception ex) when (ex is SshConnectionException or SocketException or ProxyException)
        {
            _logger.LogError(ex, "Ошибка подключения к серверу по Sftp.");
            throw;
        }

        catch (SshAuthenticationException ex)
        {
            _logger.LogError(ex, "Ошибка аутентификации к серверу по Sftp.");
            throw;
        }

        catch (SftpPermissionDeniedException ex)
        {
            _logger.LogError(ex, "Ошибка доступа к серверу по Sftp.");
            throw;
        }

        catch (SshException ex)
        {
            _logger.LogError(ex, "Ошибка Sftp.");
            throw;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке файла на сервер.");
            throw;
        }
        
        finally
        {
            sftpClient.Disconnect();
        }
    }

    /// <inheritdoc />
    public async Task<FileContentResult> DownloadFileAsync(string fileName, long projectId, long taskId)
    {
        var settings = await _globalConfigRepository.Value.GetFileManagerSettingsAsync();
        
        using var sftpClient = new SftpClient(settings.Host, settings.Port, settings.Login, settings.Password);

        try
        {
            sftpClient.Connect();

            if (!sftpClient.IsConnected)
            {
                throw new InvalidOperationException("Sftp клиент не подключен. Невозможно скачать файл.");
            }

            var sftpTaskPath = settings.SftpTaskPath;

            // Путь к файлам задач проекта.
            var userProjectPath = string.Concat(sftpTaskPath, projectId);
            var userProjectTaskPath = userProjectPath + "/" + taskId + "/";

            _logger.LogInformation($"Скачивается файл {0} ({1:N0} байт)", fileName);

            var path = userProjectTaskPath + Path.GetFileName(fileName);
            var remotePath = Path.Combine(sftpClient.WorkingDirectory, path);
            using var stream = new MemoryStream();

            sftpClient.DownloadFile(remotePath, stream);
            
            // Сбрасываем позицию на 0, иначе у файла будет размер 0 байтов,
            // если не сбросить указатель позиции в начало.
            stream.Seek(0, SeekOrigin.Begin);
            
            _logger.LogInformation($"Файл {0} ({1:N0} байт) успешно скачан.", fileName);

            var byteData = await GetByteArrayAsync(stream);
            var result = new FileContentResult(byteData, "application/octet-stream");

            return result;
        }

        catch (Exception ex) when (ex is SshConnectionException or SocketException or ProxyException)
        {
            _logger.LogError(ex, "Ошибка подключения к серверу по Sftp.");
            throw;
        }

        catch (SshAuthenticationException ex)
        {
            _logger.LogError(ex, "Ошибка аутентификации к серверу по Sftp.");
            throw;
        }

        catch (SftpPermissionDeniedException ex)
        {
            _logger.LogError(ex, "Ошибка доступа к серверу по Sftp.");
            throw;
        }

        catch (SshException ex)
        {
            _logger.LogError(ex, "Ошибка Sftp.");
            throw;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при скачивании файла с сервера.");
            throw;
        }

        finally
        {
            sftpClient.Disconnect();
            sftpClient.Dispose();
        }
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
}