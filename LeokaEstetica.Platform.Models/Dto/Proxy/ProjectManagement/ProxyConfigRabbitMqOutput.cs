namespace LeokaEstetica.Platform.Models.Dto.Proxy.ProjectManagement;

/// <summary>
/// Класс выходной модели прокси настроек RabbitMQ. 
/// </summary>
public class ProxyConfigRabbitMqOutput
{
    /// <summary>
    /// Конструктор.
    /// <param name="hostName">Название хоста.</param>
    /// <param name="userName">Логин пользователя.</param>
    /// <param name="password">Пароль.</param>
    /// <param name="virtualHost">Название виртуального хоста.</param>
    /// </summary>
    public ProxyConfigRabbitMqOutput(string? hostName, string? userName, string? password, string? virtualHost)
    {
        HostName = hostName;
        UserName = userName;
        Password = password;
    }

    /// <summary>
    /// Название хоста.
    /// </summary>
    public string? HostName { get; set; }

    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Пароль.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Название виртуального хоста.
    /// </summary>
    public string? VirtualHost { get; set; }
}