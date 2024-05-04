using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Sockets;
using LeokaEstetica.Platform.Database.Abstractions.Config;
using LeokaEstetica.Platform.Models.Enums;
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
    /// Кол-во уже сделанных попыток подключения к SFTP-серверу.
    /// </summary>
    private readonly int _retryConnections = 0;
    
    /// <summary>
    /// Кол-в овозможных повторных подключений к SFTP-серверу до 10 попыток.
    /// </summary>
    private readonly int _availableRetryConnections = 10;

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
            using var stream = new MemoryStream();

            sftpClient.DownloadFile(path, stream);
            
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
        }
    }

    /// <inheritdoc />
    public async Task RemoveFileAsync(string fileName, long projectId, long taskId)
    {
        var settings = await _globalConfigRepository.Value.GetFileManagerSettingsAsync();
        
        using var sftpClient = new SftpClient(settings.Host, settings.Port, settings.Login, settings.Password);
        
        try
        {
            sftpClient.Connect();
            
            var sftpTaskPath = settings.SftpTaskPath;

            // Путь к файлам задач проекта.
            var userProjectPath = string.Concat(sftpTaskPath, projectId);
            var userProjectTaskPath = userProjectPath + "/" + taskId + "/";
            
            // Удаляем файл с сервера.
            await sftpClient.DeleteFileAsync(userProjectTaskPath + fileName, default);
            
            // Проверяем, сколько файлов осталось у задачи на сервере.
            // IsRegularFile - определяет только наши нужные файлы.
            // Также бывают файлы системные, у них названия "." или ".." Такие исключаем через этот признак.
            var taskFiles = sftpClient.ListDirectory(userProjectTaskPath).Where(f => f.IsRegularFile);

            // Если файлов не осталось, то удаляем папку задачи проекта.
            if (!taskFiles.Any())
            {
                sftpClient.DeleteDirectory(userProjectTaskPath);
            }
            
            // Проверяем, есть ли у проекта папки задач.
            // IsRegularFile - определяет только наши нужные файлы.
            // Также бывают файлы системные, у них названия "." или ".." Такие исключаем через этот признак.
            var projectTaskFolders = sftpClient.ListDirectory(userProjectPath).Where(f => f.IsRegularFile);

            // Если папок не осталось, то удаляем папку файлов проекта.
            if (!projectTaskFolders.Any())
            {
                sftpClient.DeleteDirectory(userProjectPath);
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
            _logger.LogError(ex, "Ошибка при удалении файла с сервера.");
            throw;
        }

        finally
        {
            sftpClient.Disconnect();
        }
    }

    /// <inheritdoc />
    public async Task<FileContentResult> GetUserAvatarFileAsync(string fileName, long projectId, long? userId,
        bool isNoPhoto)
    {
        var settings = await _globalConfigRepository.Value.GetFileManagerSettingsAsync();
        
        using var sftpClient = new SftpClient(settings.Host, settings.Port, settings.Login, settings.Password);

        try
        {
            FileContentResult result;
            
            // Помещено в do, while для повторных попыток подключений к серверу.
            // Иногда от SFTP-сервера не приходят заголовки, и тогда бахает ошибка подключения.
            // Повторные попытки подключения решают эту проблему.
            // Подключение с SFTP-сервером работает на основе сокетов.
            do
            {
                sftpClient.Connect();
            
                if (!sftpClient.IsConnected)
                {
                    throw new InvalidOperationException(
                        "Sftp клиент не подключен. Невозможно скачать файл изображения аватара пользователя с сервера.");
                }
            
                var sftpAvatarPath = string.Concat(settings.SftpUserAvatarPath, "/");

                // Путь к изображениям аватара пользователей проекта.
                var userProjectPath = string.Concat(sftpAvatarPath, projectId);
                string userProjectAvatarPath;

                // Если нужно подгрузить дефолтное изображение аватара.
                if (isNoPhoto || !userId.HasValue)
                {
                    userProjectAvatarPath = sftpAvatarPath;
                }

                else
                {
                    userProjectAvatarPath = userProjectPath + "/" + userId + "/";
                }
            
                _logger.LogInformation($"Скачивается файл {0} ({1:N0} байт)", fileName);

                var path = userProjectAvatarPath + Path.GetFileName(fileName);
                using var stream = new MemoryStream();

                sftpClient.DownloadFile(path, stream);
            
                // Сбрасываем позицию на 0, иначе у файла будет размер 0 байтов,
                // если не сбросить указатель позиции в начало.
                stream.Seek(0, SeekOrigin.Begin);
            
                _logger.LogInformation($"Файл {0} ({1:N0} байт) успешно скачан.", fileName);

                var byteData = await GetByteArrayAsync(stream);
                result = new FileContentResult(byteData, "application/octet-stream");
            }
            
            // Подключаться повторно, пока не исчерпаем доступные подключения.
            while (_retryConnections < _availableRetryConnections && !sftpClient.IsConnected);
            
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
            _logger.LogError(ex, "Ошибка при получении файла изображения аватара пользователя с сервера.");
            throw;
        }

        finally
        {
            sftpClient.Disconnect();
        }
    }

    /// <inheritdoc />
    public async Task<IDictionary<long, FileContentResult>> GetUserAvatarFilesAsync(
        IEnumerable<(long? UserId, string DocumentName, DocumentTypeEnum DocumentType)> documents, long projectId)
    {
        var settings = await _globalConfigRepository.Value.GetFileManagerSettingsAsync();
        
        using var sftpClient = new SftpClient(settings.Host, settings.Port, settings.Login, settings.Password);
        
        try
        {
            sftpClient.Connect();
            
            if (!sftpClient.IsConnected)
            {
                throw new InvalidOperationException(
                    "Sftp клиент не подключен. Невозможно скачать файл изображения аватара пользователя с сервера.");
            }
            
            var sftpAvatarPath = string.Concat(settings.SftpUserAvatarPath, "/");

            // Путь к изображениям аватара пользователей проекта.
            var userProjectPath = string.Concat(sftpAvatarPath, projectId);

            var result = new Dictionary<long, FileContentResult>();

            // Скачиваем аватар пользователей с сервера.
            foreach (var d in documents)
            {
                var userProjectAvatarPath = userProjectPath + "/" + d.UserId!.Value + "/";
            
                _logger.LogInformation($"Скачивается файл {0} ({1:N0} байт)", d.DocumentName);

                var path = userProjectAvatarPath + Path.GetFileName(d.DocumentName);
                using var stream = new MemoryStream();
                
                sftpClient.DownloadFile(path, stream);
            
                // Сбрасываем позицию на 0, иначе у файла будет размер 0 байтов,
                // если не сбросить указатель позиции в начало.
                stream.Seek(0, SeekOrigin.Begin);
            
                _logger.LogInformation($"Файл {0} ({1:N0} байт) успешно скачан.", d.DocumentName);

                var byteData = await GetByteArrayAsync(stream);
                var content = new FileContentResult(byteData, "application/octet-stream");
                
                result.TryAdd(d.UserId!.Value, content);
            }

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
            _logger.LogError(ex, "Ошибка при получении файлов изображений аватара пользователей с сервера.");
            throw;
        }

        finally
        {
            sftpClient.Disconnect();
        }
    }

    /// <inheritdoc />
    public async Task UploadUserAvatarFileAsync(IFormFileCollection files, long projectId, long userId)
    {
        var settings = await _globalConfigRepository.Value.GetFileManagerSettingsAsync();
        
        using var sftpClient = new SftpClient(settings.Host, settings.Port, settings.Login, settings.Password);
        
        try
        {
            sftpClient.Connect();
            
            if (!sftpClient.IsConnected)
            {
                throw new InvalidOperationException(
                    "Sftp клиент не подключен. Невозможно загрузить файл изображения аватара пользователя на сервер.");
            }
            
            var sftpAvatarPath = string.Concat(settings.SftpUserAvatarPath, "/");

            // Путь к изображениям аватара пользователей проекта.
            var userProjectPath = string.Concat(sftpAvatarPath, projectId);
            var userProjectAvatarPath = userProjectPath + "/" + userId + "/";
            
            // Если папка пользователя у проекта не существует, то создаем ее.
            if (!sftpClient.Exists(userProjectAvatarPath))
            {
                sftpClient.CreateDirectory(userProjectAvatarPath);
            }
            
            var fileName = files.First().FileName;
            var path = userProjectAvatarPath + Path.GetFileName(fileName);
            
            var stream = await ResizeImageFromFormFileAsync(files.First());

            if (stream is null)
            {
                throw new InvalidOperationException(
                    $"Не удалось изменить размер изображения {fileName} для сохранения на сервере. " +
                    "Stream был null. " + 
                    $"ProjectId: {projectId}. " +
                    $"UserId: {userId}");
            }
            
            var fileStreamLength = stream.Length;
                
            _logger.LogInformation($"Загружается файл {0} ({1:N0} байт)", fileName, fileStreamLength);
            
            sftpClient.UploadFile(stream, path);
                
            _logger.LogInformation($"Файл {0} ({1:N0} байт) успешно загружен.", fileName, fileStreamLength);
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
            _logger.LogError(ex, "Ошибка при загрузке файла изображения аватара пользователя на сервер.");;
            throw;
        }
        
        finally
        {
            sftpClient.Disconnect();
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
    
    /// <summary>
    /// Метод изменяет размер изорбажения 50 на 50, чтобы уменьшить его размер.
    /// </summary>
    /// <param name="data">Массив байт.</param>
    /// <returns>Уменьшенный массив байт.</returns>
    // private byte[] Resize(byte[] bytes)
    // {
    //     using var stream = new MemoryStream(bytes);
    //     var image = Image.FromStream(stream);
    //
    //     var height = (50 * image.Height) / image.Width;
    //     var thumbnail = image.GetThumbnailImage(50, 50, null, IntPtr.Zero);
    //
    //     using var thumbnailStream = new MemoryStream();
    //     thumbnail.Save(thumbnailStream, ImageFormat.Jpeg);
    //     return thumbnailStream.ToArray();
    // }
    
    
    private async Task<MemoryStream> ResizeImageFromFormFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        
        using var image = Image.FromStream(memoryStream);
        var thumbnail = image.GetThumbnailImage(50, 50, null, IntPtr.Zero);

        using var thumbnailStream = new MemoryStream();
        thumbnail.Save(thumbnailStream, ImageFormat.Jpeg);
        
        return thumbnailStream;
    }
}