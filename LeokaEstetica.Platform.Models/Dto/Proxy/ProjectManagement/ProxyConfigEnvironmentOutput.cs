namespace LeokaEstetica.Platform.Models.Dto.Proxy.ProjectManagement;

/// <summary>
/// Класс выходной модели прокси настроек среды окружения. 
/// </summary>
public class ProxyConfigEnvironmentOutput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="environment">Среда окружения.</param>
    public ProxyConfigEnvironmentOutput(string environment)
    {
        Environment = environment;
    }

    /// <summary>
    /// Среда окружения.
    /// </summary>
    public string Environment { get; set; }
}