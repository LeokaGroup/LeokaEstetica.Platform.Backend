namespace LeokaEstetica.Platform.Models.Dto.Base.ProjectManagement;

/// <summary>
/// Базовый класс эпика.
/// </summary>
public class BaseEpicOutput
{
    /// <summary>
    /// PK.
    /// Id эпика.
    /// </summary>
    public long EpicId { get; set; }

    /// <summary>
    /// Название эпика.
    /// </summary>
    public string EpicName { get; set; }
}