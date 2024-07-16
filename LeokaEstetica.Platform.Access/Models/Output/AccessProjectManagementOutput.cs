namespace LeokaEstetica.Platform.Access.Models.Output;

/// <summary>
/// Класс выходной модели проверки доступа к модулям платформы.
/// </summary>
public class AccessProjectManagementOutput
{
    /// <summary>
    /// Признак наличия доступа к модулю УП.
    /// </summary>
    public bool IsAccess { get; set; }

    /// <summary>
    /// Заголовок запрета.
    /// </summary>
    public string? ForbiddenTitle { get; set; }
    
    /// <summary>
    /// Описание запрета.
    /// </summary>
    public string? ForbiddenDetails { get; set; }
}