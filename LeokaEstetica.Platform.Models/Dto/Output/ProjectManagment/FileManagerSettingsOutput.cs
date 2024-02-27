namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment;

/// <summary>
/// Класс выходной модели настроек сервиса работы с файлами.
/// </summary>
public class FileManagerSettingsOutput
{
    /// <summary>
    /// Хост.
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Порт.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Логин.
    /// </summary>
    public string Login { get; set; }

    /// <summary>
    /// Пароль.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Путь к файлам задач на сервере.
    /// </summary>
    public string SftpTaskPath { get; set; }

    /// <summary>
    /// Путь к файлам Wiki на сервере.
    /// </summary>
    public string SftpWikiPath { get; set; }

    /// <summary>
    /// Путь к файлам изображений аватара пользователей на сервере.
    /// </summary>
    public string SftpUserAvatarPath { get; set; }
}